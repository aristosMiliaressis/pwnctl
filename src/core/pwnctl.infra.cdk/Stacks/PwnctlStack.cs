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
            Vpc = new Vpc(this, Constants.VpcId, new VpcProps
            {
                MaxAzs = 2,
                NatGateways = 0,
                SubnetConfiguration = new SubnetConfiguration[]
                {
                    new SubnetConfiguration
                    {
                        Name = Constants.PublicSubnet1,
                        SubnetType = SubnetType.PUBLIC,
                        CidrMask = 24
                    },
                    new SubnetConfiguration
                    {
                        Name = Constants.PrivateSubnet1,
                        SubnetType = SubnetType.PRIVATE_ISOLATED,
                        CidrMask = 24
                    }
                }
            });
        }

        internal void CreateDatabase(bool aurora = false)
        {
            DatabaseSecret = new SecretsManager.Secret(this, Constants.DatabaseCredSecret, new SecretProps
            {
                SecretName = Constants.DatabaseCredSecretName,
                RemovalPolicy = RemovalPolicy.DESTROY,
                GenerateSecretString = new SecretStringGenerator
                {
                    SecretStringTemplate = JsonSerializer.Serialize(new { username = Constants.AuroraInstanceUsername }),
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

                Database = new DatabaseCluster(this, Constants.AuroraCluster, new DatabaseClusterProps
                {
                    ClusterIdentifier = Constants.AuroraCluster,
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
                    InstanceIdentifierBase = Constants.AuroraInstance,
                    CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
                    DefaultDatabaseName = Constants.DatabaseName,
                });

                return;
            }

            Database = new DatabaseInstance(this, Constants.AuroraInstance, new DatabaseInstanceProps
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
                InstanceIdentifier = Constants.AuroraInstance,
                CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
                DatabaseName = Constants.DatabaseName
            });
        }

        internal void CreateQueues()
        {
            DLQueue = new Queue(this, Constants.DLQName, new QueueProps
            {
                QueueName = Constants.DLQName,
                Encryption = QueueEncryption.SQS_MANAGED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(Constants.QueueVisibilityTimeoutInSec)
            });

            Queue = new Queue(this, Constants.QueueName, new QueueProps
            {
                QueueName = Constants.QueueName,
                Encryption = QueueEncryption.SQS_MANAGED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(Constants.QueueVisibilityTimeoutInSec),
                DeadLetterQueue = new DeadLetterQueue
                {
                    MaxReceiveCount = 3,
                    Queue = DLQueue
                }
            });
        }

        internal void CreateFileSystem()
        {
            FileSystem = new EFS.FileSystem(this, Constants.EfsId, new FileSystemProps
            {
                Vpc = Vpc,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            AccessPoint = FileSystem.AddAccessPoint(Constants.EfsApId, new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
                PosixUser = new PosixUser { Gid = "0", Uid = "0" },
                Path = "/"
            });
        }

        internal void CreateLambda()
        {
            var pwnctlApiRole = new Role(this, Constants.LambdaRole, new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
            Queue.GrantSendMessages(pwnctlApiRole);

            var function = new Function(this, Constants.LambdaName, new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                MemorySize = 3072,
                Timeout = Duration.Seconds(120),
                Code = Code.FromAsset(Path.Join("src", "pwnctl.api", "bin", "Release", "net7.0")),
                Handler = "pwnctl.api",
                Vpc = Vpc,
                Role = pwnctlApiRole,
                Filesystem = Lambda.FileSystem.FromEfsAccessPoint(AccessPoint, Constants.EfsMountPoint),
                LogRetention = RetentionDays.ONE_WEEK,
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_Aws__InVpc", "true"},
                    {"PWNCTL_TaskQueue__QueueName", Constants.QueueName},
                    {"PWNCTL_TaskQueue__DLQName", Constants.DLQName},
                    {"PWNCTL_TaskQueue__VisibilityTimeout", Constants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__MinLevel", "Debug"},
                    {"PWNCTL_Logging__FilePath", "/mnt/efs/"},
                    {"PWNCTL_Logging__LogGroup", Constants.LambdaLogGroup},
                    {"PWNCTL_INSTALL_PATH", Constants.EfsMountPoint}
                }
            });
            Database.Connections.AllowDefaultPortFrom(function);

            var fnUrl = function.AddFunctionUrl(new FunctionUrlOptions
            {
                AuthType = FunctionUrlAuthType.AWS_IAM
            });

            new StringParameter(this, Constants.ApiUrlParamId, new StringParameterProps
            {
                ParameterName = Constants.ApiUrlParam,
                StringValue = fnUrl.Url,
                Description = $"the base url of {Constants.LambdaName}"
            });
        }

        internal void CreateFargateService()
        {
            CreateFargateTaskDefinition();

            var cluster = new Cluster(this, Constants.EcsClusterName, new ClusterProps
            {
                Vpc = Vpc
            });

            FargateService = new FargateService(this, Constants.FargateServiceId, new FargateServiceProps
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
            var ecsTaskExecutionRole = new Role(this, Constants.EcsRoleName, new RoleProps
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

            TaskDefinition = new FargateTaskDefinition(this, Constants.TaskDefinitionId, new FargateTaskDefinitionProps
            {
                Cpu = 512,
                MemoryLimitMiB = 3072,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole,
                Volumes = new ECS.Volume[]
                {
                    new ECS.Volume()
                    {
                        Name = Constants.EfsId,
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

            var logGroup = new LogGroup(this, Constants.EcsLogGroupId, new LogGroupProps
            {
                LogGroupName = Constants.EcsLogGroupName,
                RemovalPolicy = RemovalPolicy.DESTROY,
                Retention = RetentionDays.ONE_WEEK
            });

            logGroup.GrantWrite(new ServicePrincipal("ecs-tasks.amazonaws.com"));

            var container = TaskDefinition.AddContainer(Constants.ContainerName, new ContainerDefinitionOptions
            {
                ContainerName = Constants.ContainerName,
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
                    {"PWNCTL_TaskQueue__VisibilityTimeout", Constants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__FilePath", "/mnt/efs/"},
                    {"PWNCTL_Logging__MinLevel", "Debug"},
                    {"PWNCTL_Logging__LogGroup", logGroup.LogGroupName},
                    {"PWNCTL_INSTALL_PATH", Constants.EfsMountPoint}
                }
            });

            var mountPoint = new MountPoint
            {
                SourceVolume = Constants.EfsId,
                ContainerPath = Constants.EfsMountPoint,
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

            var scaling = FargateService.AutoScaleTaskCount(new EnableScalingProps { MinCapacity = 0, MaxCapacity = Constants.EcsInstances });

            List<IScalingInterval> scalingSteps = new();
            scalingSteps.Add(new ScalingInterval
            {
                Upper = 1,
                Change = 0
            });

            for (int i = 0; i < Constants.EcsInstances / 3; i++)
            {
                scalingSteps.Add(new ScalingInterval
                {
                    Lower = 10 * i + 1,
                    Change = (i+1) * 3 + (Constants.EcsInstances % 3)
                });
            }

            scaling.ScaleOnMetric(Constants.ScaleOutPolicy, new BasicStepScalingPolicyProps
            {
                Metric = allMessages,
                AdjustmentType = AdjustmentType.EXACT_CAPACITY,
                ScalingSteps = scalingSteps.ToArray()
            });
        }
    }
}
