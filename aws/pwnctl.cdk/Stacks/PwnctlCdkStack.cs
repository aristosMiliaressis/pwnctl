using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.EFS;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.ApplicationAutoScaling;
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

            // create the VPC
            var vpc = new Vpc(this, "pwnwrk-vpc", new VpcProps { 
                MaxAzs = 1,
                NatGateways = 0
            });

            // Create a file system in EFS to store information
            var fs = new Amazon.CDK.AWS.EFS.FileSystem(this, "pwnwrk-fs", new FileSystemProps
            {
                Vpc = vpc,
                RemovalPolicy = RemovalPolicy.DESTROY
            });

            // Create a access point to EFS
            var accessPoint = fs.AddAccessPoint("pwnctl-lambda-api-fsap", new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
                Path = "/",
                PosixUser = new PosixUser { Gid = "0", Uid = "0" }
            });

            // Create Lambda role
            var pwnctlApiRole = new Role(this, "pwnctl-api-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));

            // Create the lambda function
            var function = new Function(this, "pwnctl-api", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset(Path.Join("src", "pwnctl.api", "bin", "Release", "net6.0")),
                Handler = "pwnctl.api",
                Vpc = vpc,
                Role = pwnctlApiRole,
                Filesystem = Amazon.CDK.AWS.Lambda.FileSystem.FromEfsAccessPoint(accessPoint, "/mnt/efs")
            });

            var fnUrl = function.AddFunctionUrl(new FunctionUrlOptions
            {
                AuthType = FunctionUrlAuthType.NONE
            });

            new CfnOutput(this, "PwnctlApiUrl", new CfnOutputProps
            {
                // The .url attributes will return the unique Function URL
                Value = fnUrl.Url
            });

            // SQS
            var dlq = new Queue(this, "pwnwrk-dlq.fifo", new QueueProps {
                QueueName = "pwnwrk-dlq.fifo",
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(14),
                VisibilityTimeout = Duration.Seconds(30)
            });

            var queue = new Queue(this, "pwnwrk.fifo", new QueueProps {
                QueueName = "pwnwrk.fifo",
                Encryption = QueueEncryption.UNENCRYPTED,
                ContentBasedDeduplication = true,
                MaxMessageSizeBytes = 8192,
                ReceiveMessageWaitTime = Duration.Seconds(20),
                RetentionPeriod = Duration.Days(1),
                VisibilityTimeout = Duration.Seconds(30),
                DeadLetterQueue = new DeadLetterQueue {
                    MaxReceiveCount = 10,
                    Queue = dlq
                }
            });

            // Create an ECS cluster
            var cluster = new Cluster(this, "pwnwrk-cluster", new ClusterProps {
                Vpc = vpc
            });

            var ecsTaskExecutionRole = new Role(this, "pwnwrk-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });

            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonECSTaskExecutionRolePolicy"));
            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
            queue.GrantConsumeMessages(ecsTaskExecutionRole);
            queue.GrantSendMessages(ecsTaskExecutionRole);

            // ECS Fargate Task Definition
            var fargateTaskDefinition = new FargateTaskDefinition(this, "pwnwrk-def", new FargateTaskDefinitionProps {
                MemoryLimitMiB = 2048,
                Cpu = 512,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole
            });

            fargateTaskDefinition.AddContainer("pwnwrk", new ContainerDefinitionOptions
            {
                ContainerName = "pwnwrk",
                Cpu = 512,
                MemoryLimitMiB = 2048,
                Image = ContainerImage.FromRegistry("public.ecr.aws/i0m2p7r6/pwnwrk:latest"),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps { StreamPrefix = "pwnwrk-log" }),
                Environment = new Dictionary<string, string>()
                {
                    {"PWNCTL_JobQueue__QueueName", "pwnwrk.fifo"},
                    {"PWNCTL_JobQueue__DLQName", "pwnwrk-dlq.fifo"},
                    {"PWNCTL_JobQueue__IsSQS", "true"},
                    {"PWNCTL_JobQueue__VisibilityTimeout", "30"},
                    {"PWNCTL_Db__ConnectionString", connectionString.ValueAsString},
                    {"PWNCTL_EFS_MOUNT_POINT", "/mnt/efs"}
                }
            });

            var volume = new Amazon.CDK.AWS.ECS.Volume
            {
                Name = "pwnwrk-fs",
                EfsVolumeConfiguration = new EfsVolumeConfiguration
                {
                    FileSystemId = fs.FileSystemId,
                    AuthorizationConfig = new AuthorizationConfig
                    {
                        //Iam = "DISABLED",
                        AccessPointId = accessPoint.AccessPointId
                    },
                    RootDirectory = "/",
                    TransitEncryption = "ENABLED"
                }
            };

            fargateTaskDefinition.AddVolume(volume);

            // Instantiate an Amazon ECS Service
            var fargateService = new FargateService(this, "pwnwrk-svc", new FargateServiceProps {
                Cluster = cluster,
                TaskDefinition = fargateTaskDefinition,
                DesiredCount = 0,
                CapacityProviderStrategies = new [] { new CapacityProviderStrategy {
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
                    { "QueueName", "pwnwrk.fifo" }
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
