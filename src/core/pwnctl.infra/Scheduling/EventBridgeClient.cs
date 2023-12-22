using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Amazon.EC2;
using Amazon.ECS;
using Amazon.IdentityManagement;
using pwnctl.app;
using pwnctl.app.Operations.Entities;
using pwnctl.app.Operations.Interfaces;
using pwnctl.kernel;

namespace pwnctl.infra.Scheduling;

public class EventBridgeClient : OperationStateSubscriptionService
{
    public async Task Schedule(Operation op)
    {
        var client = new AmazonCloudWatchEventsClient();
        var ecsClient = new AmazonECSClient();
        var ec2Client = new AmazonEC2Client();
        var iamClient = new AmazonIdentityManagementServiceClient();

        var subnets = await ec2Client.DescribeSubnetsAsync();

        var cluster = await ecsClient.ListClustersAsync(new());
        var clusterArn = cluster.ClusterArns.First();

        var taskDefinition = await ecsClient.ListTaskDefinitionsAsync(new Amazon.ECS.Model.ListTaskDefinitionsRequest
        {
            FamilyPrefix = "pwnctl-proc"
        });
        var taskDefinitionArn = taskDefinition.TaskDefinitionArns.First();

        var roles = await iamClient.ListRolesAsync();
        var roleArn = roles.Roles.First(r => r.RoleName == "event_publisher").Arn;

        // if no schedule provided, schedule immediatly (i.e in 2 minutes)
        var schedule = op.Schedule is null
                     ? SystemTime.UtcNow().AddMinutes(2).ToString("m H d M ? yyyy")
                     : $"{op.Schedule.Value} *";

        await client.PutRuleAsync(new PutRuleRequest
        {
            Name = $"{op.Name.Value}_schedule",
            ScheduleExpression = $"cron({schedule})"
        });

        var respone = await client.PutTargetsAsync(new PutTargetsRequest
        {
            Rule = $"{op.Name.Value}_schedule",
            Targets = new List<Target>
            {
                new Target
                {
                    Id = $"{op.Name.Value}_target",
                    Arn = clusterArn,
                    RoleArn = roleArn,
                    EcsParameters = new EcsParameters
                    {
                        TaskCount = 1,
                        TaskDefinitionArn = taskDefinitionArn,
                        LaunchType = Amazon.CloudWatchEvents.LaunchType.FARGATE,
                        NetworkConfiguration = new()
                        {
                            AwsvpcConfiguration = new()
                            {
                                AssignPublicIp = Amazon.CloudWatchEvents.AssignPublicIp.ENABLED,
                                Subnets = subnets.Subnets
                                                .Where(s => s.Tags.Any(t => t.Key == "Name" && t.Value.Contains("Public")))
                                                .Select(n => n.SubnetId)
                                                .ToList()
                            }
                        }
                    },
                    Input = $$$"""{"containerOverrides":[{"name":"pwnctl","environment":[{"name":"PWNCTL_Operation","value":"{{{op.Id}}}"}]}]}"""
                }
            }
        });

        respone.FailedEntries.ForEach(fail => PwnInfraContext.Logger.Error($"{fail.ErrorCode}:{fail.ErrorMessage}"));
    }

    public async Task DisableSchedule(Operation op)
    {
        var client = new AmazonCloudWatchEventsClient();

        try
        {
            await client.RemoveTargetsAsync(new RemoveTargetsRequest
            {
                Rule = $"{op.Name.Value}_schedule",
                Ids = new() { $"{op.Name.Value}_target" }
            });

            await client.DeleteRuleAsync(new DeleteRuleRequest
            {
                Name = $"{op.Name.Value}_schedule"
            });
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);
        }
    }

    public async Task Subscribe(Operation op) 
    {
        var client = new AmazonCloudWatchEventsClient();
        var ecsClient = new AmazonECSClient();
        var ec2Client = new AmazonEC2Client();
        var iamClient = new AmazonIdentityManagementServiceClient();

        var subnets = await ec2Client.DescribeSubnetsAsync();

        var cluster = await ecsClient.ListClustersAsync(new());
        var clusterArn = cluster.ClusterArns.First();

        var taskDefinition = await ecsClient.ListTaskDefinitionsAsync(new Amazon.ECS.Model.ListTaskDefinitionsRequest
        {
            FamilyPrefix = "pwnctl-proc"
        });
        var taskDefinitionArn = taskDefinition.TaskDefinitionArns.First();

        var roles = await iamClient.ListRolesAsync();
        var roleArn = roles.Roles.First(r => r.RoleName == "event_publisher").Arn;

        var respone = await client.PutTargetsAsync(new PutTargetsRequest
        {
            Rule = "all-tasks-completed",
            Targets = new List<Target>
            {
                new Target
                {
                    Id = $"{op.Name.Value}_target",
                    Arn = clusterArn,
                    RoleArn = roleArn,
                    EcsParameters = new EcsParameters
                    {
                        TaskCount = 1,
                        TaskDefinitionArn = taskDefinitionArn,
                        LaunchType = Amazon.CloudWatchEvents.LaunchType.FARGATE,
                        NetworkConfiguration = new()
                        {
                            AwsvpcConfiguration = new()
                            {
                                AssignPublicIp = Amazon.CloudWatchEvents.AssignPublicIp.ENABLED,
                                Subnets = subnets.Subnets
                                                .Where(s => s.Tags.Any(t => t.Key == "Name" && t.Value.Contains("Public")))
                                                .Select(n => n.SubnetId)
                                                .ToList()
                            }
                        }
                    },
                    Input = $$$"""{"containerOverrides":[{"name":"pwnctl","environment":[{"name":"PWNCTL_Operation","value":"{{{op.Id}}}"}]}]}"""
                }
            }
        });

        respone.FailedEntries.ForEach(fail => PwnInfraContext.Logger.Error($"{fail.ErrorCode}:{fail.ErrorMessage}"));
    }

    public async Task Unsubscribe(Operation op) 
    {
        var client = new AmazonCloudWatchEventsClient();

        var response = await client.ListTargetsByRuleAsync(new ListTargetsByRuleRequest
        {
            Rule = "all-tasks-completed",
        });
        
        var removeResponse = await client.RemoveTargetsAsync(new RemoveTargetsRequest 
        {
            Rule = "all-tasks-completed",
            Ids = response.Targets.Select(t => t.Id).ToList()
        });

        removeResponse.FailedEntries.ForEach(fail => PwnInfraContext.Logger.Error($"{fail.ErrorCode}:{fail.ErrorMessage}"));
    }
}
