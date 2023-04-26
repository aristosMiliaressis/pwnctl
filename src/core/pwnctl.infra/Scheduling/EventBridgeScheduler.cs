using Amazon.CloudWatchEvents;
using Amazon.CloudWatchEvents.Model;
using Amazon.ECS;
using Amazon.IdentityManagement;
using pwnctl.app.Operations.Entities;

namespace pwnctl.infra.Scheduling;

public class EventBridgeScheduler
{
    public async Task ScheduleOperation(Operation op)
    {
        var client = new AmazonCloudWatchEventsClient();
        var ecsClient = new AmazonECSClient();
        var iamClient = new AmazonIdentityManagementServiceClient();

        var cluster = await ecsClient.ListClustersAsync(new());

        var taskDefinition = await ecsClient.ListTaskDefinitionsAsync(new Amazon.ECS.Model.ListTaskDefinitionsRequest
        {
            FamilyPrefix = "pwnwrk"
        });

        var roles = await iamClient.ListRolesAsync();

        await client.PutRuleAsync(new PutRuleRequest
        {
            Name = $"{op.ShortName.Value}_schedule",
            ScheduleExpression = $"cron({op.CronSchedule})"
        });

        await client.PutTargetsAsync(new PutTargetsRequest
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
                            TaskDefinitionArn = taskDefinition.TaskDefinitionArns.First()
                        },
                        Input = $$$"""{"ContainerOverrides":{"environment":[{"name":"PWNCTL_Operation","value":"{{{op.Id}}}"}]}}"""
                    }
                }
        });
    }
}