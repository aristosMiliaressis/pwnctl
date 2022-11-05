// using System.Collections.Generic;
// using Amazon.CDK;
// using Amazon.CDK.AWS.EC2;
// using Amazon.CDK.AWS.ECS;
// using Amazon.CDK.AWS.EFS;
// using Amazon.CDK.AWS.SQS;
// using Amazon.CDK.AWS.IAM;
// using Amazon.CDK.AWS.CloudWatch;
// using Amazon.CDK.AWS.ApplicationAutoScaling;
// using Amazon.CDK.AWS.Logs;
// using SecretsManager = Amazon.CDK.AWS.SecretsManager;
// using EFS = Amazon.CDK.AWS.EFS;
// using ECS = Amazon.CDK.AWS.ECS;
// using pwnwrk.infra.Aws;
// using pwnwrk.infra.Logging;

// namespace pwnwrk.infra.cdk.Stacks
// {
//     internal sealed class PwnwrkStack : Stack
//     {
//         internal PwnwrkStack(Construct scope, string id, IStackProps props = null) 
//             : base(scope, id, props)
//         {
//             #region ECS Fargate Task Definition
//             var ecsTaskExecutionRole = new Role(this, AwsConstants.EcsRoleName, new RoleProps
//             {
//                 AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
//             });

//             ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));
//             ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
//             ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"));
//             ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("SecretsManagerReadWrite"));
//             ecsTaskExecutionRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
//             {
//                 Resources = new[] { "*" },
//                 Actions = new[] { "logs:*", "cloudwatch:*" }
//             }));
//             BaseStack.Queue.GrantConsumeMessages(ecsTaskExecutionRole);
//             BaseStack.Queue.GrantSendMessages(ecsTaskExecutionRole);
//             BaseStack.DbSecret.GrantRead(ecsTaskExecutionRole);

//             var taskDef = new FargateTaskDefinition(this, AwsConstants.TaskDefinitionId, new FargateTaskDefinitionProps
//             {
//                 MemoryLimitMiB = 2048,
//                 Cpu = 512,
//                 TaskRole = ecsTaskExecutionRole,
//                 ExecutionRole = ecsTaskExecutionRole,
//                 Volumes = new ECS.Volume[]
//                 {
//                     new ECS.Volume()
//                     {
//                         Name = AwsConstants.EfsId,
//                         EfsVolumeConfiguration = new EfsVolumeConfiguration
//                         {
//                             FileSystemId = BaseStack.FileSystem.FileSystemId,
//                             AuthorizationConfig = new AuthorizationConfig
//                             {
//                                 Iam = "ENABLED",
//                                 AccessPointId = Fn.ImportValue(AwsConstants.PwnctlEfsAccessPointId)
//                             },
//                             RootDirectory = "/",
//                             TransitEncryption = "ENABLED"
//                         }
//                     }
//                 }
//             });

//             var logGroup = new LogGroup(this, AwsConstants.EcsLogGroupId, new LogGroupProps
//             {
//                 LogGroupName = AwsConstants.EcsLogGroupName,
//                 RemovalPolicy = RemovalPolicy.DESTROY,
//                 Retention = RetentionDays.ONE_WEEK
//             });

//             logGroup.GrantWrite(new ServicePrincipal("ecs-tasks.amazonaws.com"));

//             var container = taskDef.AddContainer(AwsConstants.ContainerName, new ContainerDefinitionOptions
//             {
//                 ContainerName = AwsConstants.ContainerName,
//                 Cpu = 512,
//                 MemoryLimitMiB = 2048,
//                 Image = ContainerImage.FromRegistry("public.ecr.aws/i0m2p7r6/pwnwrk:latest"),
//                 Logging = LogDriver.AwsLogs(new AwsLogDriverProps
//                 {
//                     StreamPrefix = "/aws/ecs",
//                     LogGroup = logGroup
//                 }),
//                 Environment = new Dictionary<string, string>()
//                 {
//                     {"PWNCTL_Aws__InVpc", "true"},
//                     {"PWNCTL_JobQueue__QueueName", BaseStack.Queue.QueueName},
//                     {"PWNCTL_JobQueue__DLQName", BaseStack.DLQueue.QueueName},
//                     {"PWNCTL_JobQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
//                     {"PWNCTL_Logging__Provider", LogProfile.CloudWatch.ToString()},
//                     {"PWNCTL_Logging__MinLevel", "Debug"},
//                     {"PWNCTL_Logging__LogGroup", logGroup.LogGroupName},
//                     {"PWNCTL_InstallPath", AwsConstants.EfsMountPoint}
//                 }
//             });

//             var mountPoint = new MountPoint
//             {
//                 SourceVolume = AwsConstants.EfsId,
//                 ContainerPath = AwsConstants.EfsMountPoint,
//                 ReadOnly = false
//             };

//             container.AddMountPoints(mountPoint);
//             #endregion

//             #region ECS Fargate Service
//             var cluster = new Cluster(this, AwsConstants.EcsClusterName, new ClusterProps
//             {
//                 Vpc = BaseStack.Vpc
//             });

//             var fargateService = new FargateService(this, AwsConstants.FargateServiceId, new FargateServiceProps
//             {
//                 AssignPublicIp = true,
//                 Cluster = cluster,
//                 TaskDefinition = taskDef,
//                 DesiredCount = 0,
//                 CapacityProviderStrategies = new[] {
//                     new CapacityProviderStrategy {
//                         CapacityProvider = "FARGATE",
//                         Weight = 1
//                     }
//                 }
//             });

//             BaseStack.FileSystem.Connections.AllowFrom(fargateService, Port.Tcp(2049));

//             var queueDepthMetric = new Metric(new MetricProps
//             {
//                 Namespace = "AWS/SQS",
//                 MetricName = "ApproximateNumberOfMessagesVisible",
//                 Statistic = "Average",
//                 Period = Duration.Seconds(60),
//                 DimensionsMap = new Dictionary<string, string>
//                 {
//                     { "QueueName", BaseStack.Queue.QueueName }
//                 }
//             });

//             var scaling = fargateService.AutoScaleTaskCount(new EnableScalingProps { MinCapacity = 0, MaxCapacity = 5 });

//             scaling.ScaleOnMetric(AwsConstants.ScaleInPolicy, new BasicStepScalingPolicyProps
//             {
//                 Cooldown = Duration.Seconds(300),
//                 Metric = queueDepthMetric,
//                 AdjustmentType = AdjustmentType.EXACT_CAPACITY,
//                 ScalingSteps = new[]
//                 {
//                     new ScalingInterval
//                     {
//                         Upper = 1,
//                         Change = 0
//                     },
//                     new ScalingInterval
//                     {
//                         Upper = 30,
//                         Change = 1
//                     },
//                     new ScalingInterval
//                     {
//                         Upper = 60,
//                         Change = 2
//                     }
//                 }
//             });

//             scaling.ScaleOnMetric(AwsConstants.ScaleOutPolicy, new BasicStepScalingPolicyProps
//             {
//                 Cooldown = Duration.Seconds(300),
//                 Metric = queueDepthMetric,
//                 AdjustmentType = AdjustmentType.EXACT_CAPACITY,
//                 ScalingSteps = new[]
//                 {
//                     new ScalingInterval
//                     {
//                         Lower = 60,
//                         Change = 3
//                     },
//                     new ScalingInterval
//                     {
//                         Lower = 30,
//                         Change = 2
//                     },
//                     new ScalingInterval
//                     {
//                         Lower = 1,
//                         Change = 1
//                     }
//                 }
//             });
//             #endregion
//         }
//     }
// }
