using Constructs;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.EFS;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.RDS;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SecretsManager;
using SecretsManager = Amazon.CDK.AWS.SecretsManager;
using EFS = Amazon.CDK.AWS.EFS;
using ECS = Amazon.CDK.AWS.ECS;
using RDS = Amazon.CDK.AWS.RDS;
using Lambda = Amazon.CDK.AWS.Lambda;
using pwnctl.infra.Aws;

namespace pwnctl.infra.cdk.Stacks
{
    internal sealed class PwnctlStack : Stack
    {
        internal Vpc Vpc { get; set; }
        internal IConnectable Database { get; set; }
        internal SecretsManager.Secret DatabaseSecret { get; set; }
        internal Queue Queue { get; set; }
        internal Queue DLQueue { get; set; }
        internal EFS.FileSystem FileSystem { get; set; }
        internal AccessPoint AccessPoint { get; set; }
        internal TaskDefinition TaskDefinition { get; set; }
        internal FargateService FargateService { get; set; }

        internal PwnctlStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            CreateVpc();

            CreateDatabase();

            CreateQueues();

            CreateFileSystem();

            CreateLambda();

            CreateFargateService();
        }

        internal void CreateVpc()
        {
            Vpc = new Vpc(this, AwsConstants.VpcId, new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 0,
                SubnetConfiguration = new SubnetConfiguration[]
                {
                    new SubnetConfiguration
                    {
                        Name = AwsConstants.PublicSubnet1,
                        SubnetType = SubnetType.PUBLIC,
                        CidrMask = 24
                    },
                    new SubnetConfiguration
                    {
                        Name = AwsConstants.PrivateSubnet1,
                        SubnetType = SubnetType.PRIVATE_ISOLATED,
                        CidrMask = 24
                    }
                }
            });
        }

        internal void CreateDatabase(bool aurora = false)
        {
            DatabaseSecret = new SecretsManager.Secret(this, AwsConstants.DatabaseCredSecret, new SecretProps
            {
                SecretName = AwsConstants.DatabaseCredSecretName,
                RemovalPolicy = RemovalPolicy.DESTROY,
                GenerateSecretString = new SecretStringGenerator
                {
                    SecretStringTemplate = JsonSerializer.Serialize(new { username = AwsConstants.AuroraInstanceUsername }),
                    ExcludePunctuation = true,
                    IncludeSpace = false,
                    GenerateStringKey = "password"
                }
            });

            if (aurora)
            {
                var dbEngine = DatabaseClusterEngine.AuroraPostgres(new AuroraPostgresClusterEngineProps
                {
                    Version = AuroraPostgresEngineVersion.VER_14_5
                });

                Database = new DatabaseCluster(this, AwsConstants.AuroraCluster, new DatabaseClusterProps
                {
                    ClusterIdentifier = AwsConstants.AuroraCluster,
                    Engine = dbEngine,
                    Credentials = Credentials.FromSecret(DatabaseSecret),
                    InstanceProps = new RDS.InstanceProps
                    {
                        InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                        VpcSubnets = new SubnetSelection
                        {
                            SubnetType = SubnetType.PRIVATE_ISOLATED
                        },
                        Vpc = Vpc,
                    },
                    RemovalPolicy = RemovalPolicy.DESTROY,
                    Instances = 1,
                    InstanceIdentifierBase = AwsConstants.AuroraInstance,
                    CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
                    DefaultDatabaseName = AwsConstants.DatabaseName,
                });

                return;
            }

            Database = new DatabaseInstance(this, AwsConstants.AuroraInstance, new DatabaseInstanceProps
            {
                Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps { Version = PostgresEngineVersion.VER_14_4 }),
                InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MICRO),
                Credentials = Credentials.FromSecret(DatabaseSecret),//Credentials.FromGeneratedSecret("syscdk"),
                Vpc = Vpc,
                VpcSubnets = new SubnetSelection
                {
                    SubnetType = SubnetType.PRIVATE_ISOLATED
                },
                RemovalPolicy = RemovalPolicy.DESTROY,
                InstanceIdentifier = AwsConstants.AuroraInstance,
                CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
                DatabaseName = AwsConstants.DatabaseName
            });
        }

        internal void CreateQueues()
        {
            DLQueue = new Queue(this, AwsConstants.DLQName, new QueueProps
            {
                QueueName = AwsConstants.DLQName,
                Encryption = QueueEncryption.SQS_MANAGED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec)
            });

            Queue = new Queue(this, AwsConstants.QueueName, new QueueProps
            {
                QueueName = AwsConstants.QueueName,
                Encryption = QueueEncryption.SQS_MANAGED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec),
                DeadLetterQueue = new DeadLetterQueue
                {
                    MaxReceiveCount = 3,
                    Queue = DLQueue
                }
            });
        }

        internal void CreateFileSystem()
        {
            FileSystem = new EFS.FileSystem(this, AwsConstants.EfsId, new FileSystemProps
            {
                Vpc = Vpc,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            AccessPoint = FileSystem.AddAccessPoint(AwsConstants.EfsApId, new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
                PosixUser = new PosixUser { Gid = "0", Uid = "0" },
                Path = "/"
            });
        }

        internal void CreateLambda()
        {
            var pwnctlApiRole = new Role(this, AwsConstants.LambdaRole, new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
            Queue.GrantSendMessages(pwnctlApiRole);

            var function = new Function(this, AwsConstants.LambdaName, new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                MemorySize = 3072,
                Timeout = Duration.Seconds(120),
                Code = Code.FromAsset(Path.Join("src", "pwnctl.api", "bin", "Release", "net6.0")),
                Handler = "pwnctl.api",
                Vpc = Vpc,
                Role = pwnctlApiRole,
                Filesystem = Lambda.FileSystem.FromEfsAccessPoint(AccessPoint, AwsConstants.EfsMountPoint),
                LogRetention = RetentionDays.ONE_WEEK,
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_Aws__InVpc", "true"},
                    {"PWNCTL_TaskQueue__QueueName", AwsConstants.QueueName},
                    {"PWNCTL_TaskQueue__DLQName", AwsConstants.DLQName},
                    {"PWNCTL_TaskQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__MinLevel", "Debug"},
                    {"PWNCTL_Logging__FilePath", "/mnt/efs/"},
                    {"PWNCTL_Logging__LogGroup", AwsConstants.LambdaLogGroup},
                    {"PWNCTL_InstallPath", AwsConstants.EfsMountPoint}
                }
            });
            Database.Connections.AllowDefaultPortFrom(function);

            var fnUrl = function.AddFunctionUrl(new FunctionUrlOptions
            {
                AuthType = FunctionUrlAuthType.AWS_IAM
            });

            new StringParameter(this, AwsConstants.ApiUrlParamId, new StringParameterProps
            {
                ParameterName = AwsConstants.ApiUrlParam,
                StringValue = fnUrl.Url,
                Description = $"the base url of {AwsConstants.LambdaName}"
            });
        }

        internal void CreateFargateService()
        {
            CreateFargateTaskDefinition();

            var cluster = new Cluster(this, AwsConstants.EcsClusterName, new ClusterProps
            {
                Vpc = Vpc
            });

            FargateService = new FargateService(this, AwsConstants.FargateServiceId, new FargateServiceProps
            {
                AssignPublicIp = true,
                Cluster = cluster,
                TaskDefinition = TaskDefinition,
                DesiredCount = 0,
                CapacityProviderStrategies = new[] {
                    new CapacityProviderStrategy {
                        CapacityProvider = "FARGATE",
                        Weight = 1
                    }
                }
            });

            FileSystem.Connections.AllowDefaultPortFrom(FargateService);
            Database.Connections.AllowDefaultPortFrom(FargateService);

            CreateStepScalingPolicy();
        }

        internal void CreateFargateTaskDefinition()
        {
            var ecsTaskExecutionRole = new Role(this, AwsConstants.EcsRoleName, new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });

            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("CloudWatchLogsFullAccess"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
            Queue.GrantConsumeMessages(ecsTaskExecutionRole);
            Queue.GrantSendMessages(ecsTaskExecutionRole);
            DatabaseSecret.GrantRead(ecsTaskExecutionRole);

            TaskDefinition = new FargateTaskDefinition(this, AwsConstants.TaskDefinitionId, new FargateTaskDefinitionProps
            {
                Cpu = 1024,
                MemoryLimitMiB = 8192,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole,
                Volumes = new ECS.Volume[]
                {
                    new ECS.Volume()
                    {
                        Name = AwsConstants.EfsId,
                        EfsVolumeConfiguration = new EfsVolumeConfiguration
                        {
                            FileSystemId = FileSystem.FileSystemId,
                            AuthorizationConfig = new AuthorizationConfig
                            {
                                Iam = "ENABLED",
                                AccessPointId = AccessPoint.AccessPointId
                            },
                            RootDirectory = "/",
                            TransitEncryption = "ENABLED"
                        }
                    }
                }
            });

            var logGroup = new LogGroup(this, AwsConstants.EcsLogGroupId, new LogGroupProps
            {
                LogGroupName = AwsConstants.EcsLogGroupName,
                RemovalPolicy = RemovalPolicy.DESTROY,
                Retention = RetentionDays.ONE_WEEK
            });

            logGroup.GrantWrite(new ServicePrincipal("ecs-tasks.amazonaws.com"));

            var container = TaskDefinition.AddContainer(AwsConstants.ContainerName, new ContainerDefinitionOptions
            {
                ContainerName = AwsConstants.ContainerName,
                StopTimeout = Duration.Seconds(120),
                Image = ContainerImage.FromRegistry("public.ecr.aws/i0m2p7r6/pwnctl:latest"),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps
                {
                    StreamPrefix = "/aws/ecs",
                    LogGroup = logGroup
                }),
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_Aws__InVpc", "true"},
                    {"PWNCTL_TaskQueue__QueueName", Queue.QueueName},
                    {"PWNCTL_TaskQueue__DLQName", DLQueue.QueueName},
                    {"PWNCTL_TaskQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__FilePath", "/mnt/efs/"},
                    {"PWNCTL_Logging__MinLevel", "Debug"},
                    {"PWNCTL_Logging__LogGroup", logGroup.LogGroupName},
                    {"PWNCTL_InstallPath", AwsConstants.EfsMountPoint}
                }
            });

            var mountPoint = new MountPoint
            {
                SourceVolume = AwsConstants.EfsId,
                ContainerPath = AwsConstants.EfsMountPoint,
                ReadOnly = false
            };

            container.AddMountPoints(mountPoint);
        }

        internal void CreateStepScalingPolicy()
        {
            var visibleMessages = new Metric(new MetricProps
            {
                Namespace = "AWS/SQS",
                MetricName = "ApproximateNumberOfMessagesVisible",
                Statistic = "Maximum",
                Period = Duration.Seconds(60),
                DimensionsMap = new Dictionary<string, string>
                {
                    { "QueueName", Queue.QueueName }
                }
            });

            var inFlightMessages = new Metric(new MetricProps
            {
                Namespace = "AWS/SQS",
                MetricName = "ApproximateNumberOfMessagesNotVisible",
                Statistic = "Maximum",
                Period = Duration.Seconds(60),
                DimensionsMap = new Dictionary<string, string>
                {
                    { "QueueName", Queue.QueueName }
                }
            });

            var allMessages = new MathExpression(new MathExpressionProps
            {
                Expression = "visibleMessages + inFlightMessages",
                UsingMetrics = new Dictionary<string, IMetric> {
                    { "visibleMessages", visibleMessages },
                    { "inFlightMessages", inFlightMessages }
                }
            });

            var scaling = FargateService.AutoScaleTaskCount(new EnableScalingProps { MinCapacity = 0, MaxCapacity = AwsConstants.EcsInstances });

            List<IScalingInterval> scalingSteps = new()
            {
                new ScalingInterval
                {
                    Upper = 1,
                    Change = 0
                },
                new ScalingInterval
                {
                    Upper = 10000,
                    Change = AwsConstants.EcsInstances
                }
            };

            scaling.ScaleOnMetric(AwsConstants.ScaleOutPolicy, new BasicStepScalingPolicyProps
            {
                Metric = allMessages,
                AdjustmentType = AdjustmentType.EXACT_CAPACITY,
                ScalingSteps = scalingSteps.ToArray()
            });
        }
    }
}
