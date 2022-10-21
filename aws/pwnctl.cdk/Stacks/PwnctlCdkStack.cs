using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.EFS;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using Amazon.CDK.AWS.Logs;
using System.Collections.Generic;
using System.IO;

namespace pwnctl.cdk
{
    internal class PwnctlCdkStack : Stack
    {
        internal PwnctlCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var connectionString = new CfnParameter(this, "connectionString", new CfnParameterProps
            {
                Type = "String",
                Description = "The database connection string"
            });

            var vpc = new Vpc(this, Constants.VpcId, new VpcProps { 
                MaxAzs = 1,
                NatGateways = 0
            });

            var (queue, dlq) = CreateQueues();

            var fs = new Amazon.CDK.AWS.EFS.FileSystem(this, Constants.EfsId, new FileSystemProps
            {
                Vpc = vpc,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            var accessPoint = fs.AddAccessPoint(Constants.EfsApId, new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
                Path = "/",
                PosixUser = new PosixUser { Gid = "0", Uid = "0" }
            });

            CreateLambdaApi(vpc, accessPoint);
            
            var cluster = new Cluster(this, Constants.EcsClusterName, new ClusterProps {
                Vpc = vpc
            });

            var taskDef = CreateTaskDefinition(connectionString, fs, accessPoint, queue, dlq);

            CreateFargateService(cluster, taskDef, fs, queue);
        }

        internal (Queue, Queue) CreateQueues()
        {
            var dlq = new Queue(this, Constants.DLQName, new QueueProps
            {
                QueueName = Constants.DLQName,
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(Constants.QueueVisibilityTimeoutInSec)
            });

            var queue = new Queue(this, Constants.QueueName, new QueueProps
            {
                QueueName = Constants.QueueName,
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(1),
                VisibilityTimeout = Duration.Seconds(Constants.QueueVisibilityTimeoutInSec),
                DeadLetterQueue = new DeadLetterQueue
                {
                    MaxReceiveCount = 10,
                    Queue = dlq
                }
            });

            return (queue, dlq);
        }

        public void CreateLambdaApi(Vpc vpc, AccessPoint accessPoint)
        {
            var pwnctlApiRole = new Role(this, Constants.LambdaRole, new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));

            var function = new Function(this, Constants.LambdaName, new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(Path.Join("src", "pwnctl.api", "bin", "Release", "net6.0")),
                Handler = "pwnctl.api",
                Vpc = vpc,
                Role = pwnctlApiRole,
                Filesystem = Amazon.CDK.AWS.Lambda.FileSystem.FromEfsAccessPoint(accessPoint, Constants.EfsMountPoint)
            });

            var fnUrl = function.AddFunctionUrl(new FunctionUrlOptions
            {
                AuthType = FunctionUrlAuthType.NONE
            });

            new CfnOutput(this, "PwnctlApiUrl", new CfnOutputProps
            {
                Value = fnUrl.Url
            });
        }

        public FargateTaskDefinition CreateTaskDefinition(CfnParameter connectionString, Amazon.CDK.AWS.EFS.FileSystem fs, AccessPoint accessPoint, Queue queue, Queue dlq)
        {
            var ecsTaskExecutionRole = new Role(this, Constants.EcsRoleName, new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });

            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("CloudWatchFullAccess"));            
            queue.GrantConsumeMessages(ecsTaskExecutionRole);
            queue.GrantSendMessages(ecsTaskExecutionRole);

            var taskDef = new FargateTaskDefinition(this, Constants.TaskDefinitionId, new FargateTaskDefinitionProps
            {
                MemoryLimitMiB = 2048,
                Cpu = 512,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole,
                Volumes = new Amazon.CDK.AWS.ECS.Volume[]
                {
                    new Amazon.CDK.AWS.ECS.Volume()
                    {
                        Name = Constants.EfsId,
                        EfsVolumeConfiguration = new EfsVolumeConfiguration
                        {
                            FileSystemId = fs.FileSystemId,
                            AuthorizationConfig = new AuthorizationConfig
                            {
                                Iam = "ENABLED",
                                AccessPointId = accessPoint.AccessPointId
                            },
                            RootDirectory = "/",
                            TransitEncryption = "ENABLED"
                        }
                    }
                }
            });

            var logGroup = new LogGroup(this, "PwnwrkLogs", new LogGroupProps
            {
                LogGroupName = "/aws/ecs/pwnwrk",
                RemovalPolicy = RemovalPolicy.DESTROY,
                Retention = RetentionDays.ONE_WEEK
            });

            var container = taskDef.AddContainer(Constants.ContainerName, new ContainerDefinitionOptions
            {
                ContainerName = Constants.ContainerName,
                Cpu = 512,
                MemoryLimitMiB = 2048,
                Image = ContainerImage.FromRegistry("public.ecr.aws/i0m2p7r6/pwnwrk:latest"),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps
                {
                    StreamPrefix = "/aws/ecs",
                    LogGroup = logGroup
                }),
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_JobQueue__QueueName", queue.QueueName},
                    {"PWNCTL_JobQueue__DLQName", dlq.QueueName},
                    {"PWNCTL_JobQueue__VisibilityTimeout", Constants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__LogGroup", logGroup.LogGroupName},
                    {"PWNCTL_Db__ConnectionString", connectionString.ValueAsString},
                    {"PWNCTL_EFS_MOUNT_POINT", Constants.EfsMountPoint}
                }
            });

            var mountPoint = new Amazon.CDK.AWS.ECS.MountPoint
            {
                SourceVolume = Constants.EfsId,
                ContainerPath = Constants.EfsMountPoint,
                ReadOnly = false
            };

            container.AddMountPoints(mountPoint);

            return taskDef;
        }

        internal void CreateFargateService(Cluster cluster, FargateTaskDefinition taskDef, Amazon.CDK.AWS.EFS.FileSystem fs, Queue queue)
        {
            var fargateService = new FargateService(this, Constants.FargateServiceId, new FargateServiceProps
            {
                AssignPublicIp = true,
                Cluster = cluster,
                TaskDefinition = taskDef,
                DesiredCount = 0,
                CapacityProviderStrategies = new[] { new CapacityProviderStrategy {
                    CapacityProvider = "FARGATE_SPOT",
                    Weight = 2
                }, new CapacityProviderStrategy {
                    CapacityProvider = "FARGATE",
                    Weight = 1
                } }
            });

            fs.Connections.AllowFrom(fargateService, Amazon.CDK.AWS.EC2.Port.Tcp(2049));

            var queueDepthMetric = new Metric(new MetricProps
            {
                Namespace = "AWS/SQS",
                MetricName = "ApproximateNumberOfMessagesVisible",
                Statistic = "Average",
                Period = Duration.Seconds(60),
                DimensionsMap = new Dictionary<string, string>
                {
                    { "QueueName", queue.QueueName }
                }
            });

            var scaling = fargateService.AutoScaleTaskCount(new EnableScalingProps { MinCapacity = 0, MaxCapacity = 5 });

            scaling.ScaleOnMetric("ScaleOutPolicy", new Amazon.CDK.AWS.ApplicationAutoScaling.BasicStepScalingPolicyProps
            {
                Cooldown = Duration.Seconds(300),
                Metric = queueDepthMetric,
                ScalingSteps = new[]
                {
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Upper = 1,
                        Change = 1
                    },
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Upper = 30, Change = 2
                    } ,
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Upper = 60, Change = 3
                    }
                }
            });

            scaling.ScaleOnMetric("ScaleInPolicy", new Amazon.CDK.AWS.ApplicationAutoScaling.BasicStepScalingPolicyProps
            {
                Cooldown = Duration.Seconds(300),
                Metric = queueDepthMetric,
                ScalingSteps = new[]
                {
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Lower = 60,
                        Change = 2
                    },
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Lower = 30,
                        Change = 1
                    },
                    new Amazon.CDK.AWS.ApplicationAutoScaling.ScalingInterval
                    {
                        Lower = 1,
                        Change = 0
                    }
                }
            });
        }
    }
}
