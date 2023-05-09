using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Amazon.EC2;
using Amazon.ECS;
using Amazon.IdentityManagement;
using pwnctl.app;
using pwnctl.app.Operations.Entities;
using pwnctl.kernel;

namespace pwnctl.infra.Scheduling;

public class EventBridgeScheduler
{
    public async Task ScheduleOperation(Operation op)
    {
        var client = new AmazonCloudWatchEventsClient();
        var ecsClient = new AmazonECSClient();
        var ec2Client = new AmazonEC2Client();
        var iamClient = new AmazonIdentityManagementServiceClient();

        var subnets = await ec2Client.DescribeSubnetsAsync();

        var cluster = await ecsClient.ListClustersAsync(new());

        var taskDefinition = await ecsClient.ListTaskDefinitionsAsync(new Amazon.ECS.Model.ListTaskDefinitionsRequest
        {
            FamilyPrefix = "pwnwrk"
        });

        var roles = await iamClient.ListRolesAsync();

        // if no schedule provided, schedule immediatly (i.e in 2 minutes)
        var schedule = op.Schedule == null
                     ? SystemTime.UtcNow().AddMinutes(2).ToString("m H d M ? yyyy")
                     : $"{op.Schedule.Value} *";

        await client.PutRuleAsync(new PutRuleRequest
        {
            Name = $"{op.ShortName.Value}_schedule",
            ScheduleExpression = $"cron({schedule})"
        });

        var respone = await client.PutTargetsAsync(new PutTargetsRequest
        {
            Rule = $"{op.ShortName.Value}_schedule",
            Targets = new List<Target>
            {
                new Target
                {
                    Id = $"{op.ShortName.Value}_target",
                    Arn = cluster.ClusterArns.First(),
                    RoleArn = roles.Roles.First(r => r.RoleName == "ecs_events").Arn,
                    EcsParameters = new EcsParameters
                    {
                        TaskCount = 1,
                        TaskDefinitionArn = taskDefinition.TaskDefinitionArns.First(),
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

    public async Task DisableScheduledOperation(Operation op)
    {
        var client = new AmazonCloudWatchEventsClient();

        try
        {
            await client.RemoveTargetsAsync(new RemoveTargetsRequest
            {
                Rule = $"{op.ShortName.Value}_schedule",
                Ids = new() { $"{op.ShortName.Value}_target" }
            });

            await client.DeleteRuleAsync(new DeleteRuleRequest
            {
                Name = $"{op.ShortName.Value}_schedule"
            });
        }
        catch (Exception ex)
        {
            PwnInfraContext.Logger.Exception(ex);
        }
    }
}
