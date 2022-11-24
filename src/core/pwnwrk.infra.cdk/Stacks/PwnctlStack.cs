using System;
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
using pwnwrk.infra.Aws;
using pwnwrk.infra.Logging;

namespace pwnwrk.infra.cdk.Stacks
{
    internal sealed class PwnctlStack : Stack
    {
        internal PwnctlStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            #region VPC
            var vpc = new Vpc(this, AwsConstants.VpcId, new VpcProps { 
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
            #endregion

            #region Aurora RDS
            var dbSecret = new SecretsManager.Secret(this, AwsConstants.DatabaseCredSecret, new SecretProps
            {
                SecretName = AwsConstants.DatabaseCredSecretName,
                GenerateSecretString = new SecretStringGenerator
                {
                    SecretStringTemplate = JsonSerializer.Serialize(new { username = AwsConstants.AuroraInstanceUsername }),
                    ExcludePunctuation = true,
                    IncludeSpace = false,
                    GenerateStringKey = "password"
                }
            });

            var dbEngine = DatabaseClusterEngine.AuroraPostgres(new AuroraPostgresClusterEngineProps
            {
                Version = AuroraPostgresEngineVersion.VER_13_6
            });

            var dbCluster = new DatabaseCluster(this, AwsConstants.AuroraCluster, new DatabaseClusterProps
            {
                ClusterIdentifier = AwsConstants.AuroraCluster,
                Engine = dbEngine,
                Credentials = Credentials.FromSecret(dbSecret),
                InstanceProps = new RDS.InstanceProps
                {
                    InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MEDIUM),
                    VpcSubnets = new SubnetSelection
                    {
                        SubnetType = SubnetType.PRIVATE_ISOLATED
                    },
                    Vpc = vpc,
                },
                RemovalPolicy = RemovalPolicy.DESTROY,
                Instances = 1,
                InstanceIdentifierBase = AwsConstants.AuroraInstance,
                CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
                DefaultDatabaseName = AwsConstants.DatabaseName,
            });
            #endregion

            #region SQS
            var dlq = new Queue(this, AwsConstants.DLQName, new QueueProps
            {
                QueueName = AwsConstants.DLQName,
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec)
            });

            var queue = new Queue(this, AwsConstants.QueueName, new QueueProps
            {
                QueueName = AwsConstants.QueueName,
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(1),
                VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec),
                DeadLetterQueue = new DeadLetterQueue
                {
                    MaxReceiveCount = 10,
                    Queue = dlq
                }
            });
            #endregion

            #region EFS
            var fs = new EFS.FileSystem(this, AwsConstants.EfsId, new FileSystemProps
            {
                Vpc = vpc,
                RemovalPolicy = RemovalPolicy.RETAIN
            });

            var accessPoint = fs.AddAccessPoint(AwsConstants.EfsApId, new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
                PosixUser = new PosixUser { Gid = "0", Uid = "0" },
                Path = "/"
            });
            #endregion

            #region Lambda API
            var pwnctlApiRole = new Role(this, AwsConstants.LambdaRole, new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
            queue.GrantSendMessages(pwnctlApiRole);

            // Costs 7,20$ per month
            var sqsEp = new InterfaceVpcEndpoint(this, AwsConstants.VpcSQSEndpoint, new InterfaceVpcEndpointProps
            {
                Vpc = vpc,
                Service = InterfaceVpcEndpointAwsService.SQS,
                Subnets = new SubnetSelection
                {
                    Subnets = vpc.IsolatedSubnets
                }
            });

            var function = new Function(this, AwsConstants.LambdaName, new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                MemorySize = 1024,
                Timeout = Duration.Seconds(60),
                Code = Code.FromAsset(Path.Join("src", "pwnctl", "pwnctl.api", "bin", "Release", "net6.0")),
                Handler = "pwnctl.api",
                Vpc = vpc,
                Role = pwnctlApiRole,
                Filesystem = Lambda.FileSystem.FromEfsAccessPoint(accessPoint, AwsConstants.EfsMountPoint),
                LogRetention = RetentionDays.ONE_WEEK,
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_Aws__InVpc", "true"},
                    {"PWNCTL_JobQueue__QueueName", AwsConstants.QueueName},
                    {"PWNCTL_JobQueue__DLQName", AwsConstants.DLQName},
                    {"PWNCTL_JobQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__Provider", LogProfile.Console.ToString()},
                    {"PWNCTL_Logging__MinLevel", "Debug"},
                    {"PWNCTL_Logging__LogGroup", AwsConstants.LambdaLogGroup},
                    {"PWNCTL_InstallPath", AwsConstants.EfsMountPoint}
                }
            });
            dbCluster.Connections.AllowDefaultPortFrom(function);

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
            #endregion

            #region ECS Fargate Task Definition
            var ecsTaskExecutionRole = new Role(this, AwsConstants.EcsRoleName, new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });

            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("CloudWatchLogsFullAccess"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
            queue.GrantConsumeMessages(ecsTaskExecutionRole);
            queue.GrantSendMessages(ecsTaskExecutionRole);
            dbSecret.GrantRead(ecsTaskExecutionRole);

            var taskDef = new FargateTaskDefinition(this, AwsConstants.TaskDefinitionId, new FargateTaskDefinitionProps
            {
                MemoryLimitMiB = 2048,
                Cpu = 512,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole,
                Volumes = new ECS.Volume[]
                {
                    new ECS.Volume()
                    {
                        Name = AwsConstants.EfsId,
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

            var logGroup = new LogGroup(this, AwsConstants.EcsLogGroupId, new LogGroupProps
            {
                LogGroupName = AwsConstants.EcsLogGroupName,
                RemovalPolicy = RemovalPolicy.DESTROY,
                Retention = RetentionDays.ONE_WEEK
            });

            logGroup.GrantWrite(new ServicePrincipal("ecs-tasks.amazonaws.com"));

            var container = taskDef.AddContainer(AwsConstants.ContainerName, new ContainerDefinitionOptions
            {
                ContainerName = AwsConstants.ContainerName,
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
                    {"PWNCTL_Aws__InVpc", "true"},
                    {"PWNCTL_JobQueue__QueueName", queue.QueueName},
                    {"PWNCTL_JobQueue__DLQName", dlq.QueueName},
                    {"PWNCTL_JobQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
                    {"PWNCTL_Logging__Provider", LogProfile.File.ToString()},
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
            #endregion

            #region ECS Fargate Service
            var cluster = new Cluster(this, AwsConstants.EcsClusterName, new ClusterProps
            {
                Vpc = vpc
            });

            var fargateService = new FargateService(this, AwsConstants.FargateServiceId, new FargateServiceProps
            {
                AssignPublicIp = true,
                Cluster = cluster,
                TaskDefinition = taskDef,
                DesiredCount = 0,
                CapacityProviderStrategies = new[] {
                    new CapacityProviderStrategy {
                        CapacityProvider = "FARGATE",
                        Weight = 1
                    }
                }
            });

            fs.Connections.AllowDefaultPortFrom(fargateService);
            dbCluster.Connections.AllowDefaultPortFrom(fargateService);

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

            scaling.ScaleOnMetric(AwsConstants.ScaleInPolicy, new BasicStepScalingPolicyProps
            {
                Cooldown = Duration.Seconds(300),
                Metric = queueDepthMetric,
                AdjustmentType = AdjustmentType.EXACT_CAPACITY,
                ScalingSteps = new[]
                {
                    new ScalingInterval
                    {
                        Upper = 1,
                        Change = 0
                    },
                    new ScalingInterval
                    {
                        Upper = 30,
                        Change = 1
                    },
                    new ScalingInterval
                    {
                        Upper = 60,
                        Change = 2
                    }
                }
            });

            scaling.ScaleOnMetric(AwsConstants.ScaleOutPolicy, new BasicStepScalingPolicyProps
            {
                Cooldown = Duration.Seconds(300),
                Metric = queueDepthMetric,
                AdjustmentType = AdjustmentType.EXACT_CAPACITY,
                ScalingSteps = new[]
                {
                    new ScalingInterval
                    {
                        Lower = 60,
                        Change = 3
                    },
                    new ScalingInterval
                    {
                        Lower = 30,
                        Change = 2
                    },
                    new ScalingInterval
                    {
                        Lower = 1,
                        Change = 1
                    }
                }
            });
            #endregion
        }
    }
}
