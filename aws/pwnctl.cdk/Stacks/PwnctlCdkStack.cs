using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.EFS;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.CloudWatch;
using Amazon.CDK.AWS.AutoScaling;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using System.Linq;
using System.Collections.Generic;
using System;

namespace pwnctl.cdk
{
    internal class PwnctlCdkStack : Stack
    {
        internal PwnctlCdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // create the VPC
            var vpc = new Vpc(this, "pwnwrk-vpc", new VpcProps { MaxAzs = 1 });

            // Create a file system in EFS to store information
            var fs = new Amazon.CDK.AWS.EFS.FileSystem(this, "pwnwrk-fs", new FileSystemProps
            {
                Vpc = vpc,
                RemovalPolicy = RemovalPolicy.DESTROY
            });
            
            // Create a access point to EFS
            var accessPoint = fs.AddAccessPoint("pwnctl-lambda-api-fsap", new AccessPointOptions
            {
                CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777"},
                Path = "/",
                PosixUser = new PosixUser { Gid = "0", Uid = "0" }
            });

            // Create Lambda role
            var pwnctlApiRole = new Role(this, "pwnctl-api-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
            });

            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSLambdaBasicExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AWSLambdaVPCAccessExecutionRole"));
            pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));

            // Create the lambda function
            var handler = new Function(this, "pwnctl-lambda-api", new FunctionProps
            {
                Runtime = Runtime.DOTNET_6,
                Code = Code.FromAsset("../../src/pwnctl.api"),
                Handler = "../../src/pwnctl.api",
                Vpc = vpc,
                Role = pwnctlApiRole,
                Filesystem = Amazon.CDK.AWS.Lambda.FileSystem.FromEfsAccessPoint(accessPoint, "/mnt/efs")
            });

            handler.AddFunctionUrl(new FunctionUrlOptions {
                AuthType = FunctionUrlAuthType.NONE
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

            var ecsTaskExecutionRole = new Role(this, "pwnwrk-queue-consumer-role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("ecs-tasks.amazonaws.com")
            });

            ecsTaskExecutionRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonECSTaskExecutionRolePolicy"));

            // ECS Fargate Task Definition
            var fargateTaskDefinition = new FargateTaskDefinition(this, "pwnwrk-def", new FargateTaskDefinitionProps {
                MemoryLimitMiB = 2048,
                Cpu = 512,
                TaskRole = ecsTaskExecutionRole,
                ExecutionRole = ecsTaskExecutionRole
            });

            var container = fargateTaskDefinition.AddContainer("pwnwrk", new ContainerDefinitionOptions
            {
                Image = ContainerImage.FromRegistry("public.ecr.aws/i0m2p7r6/pwnwrk:latest"),
                Logging = LogDriver.AwsLogs(new AwsLogDriverProps { StreamPrefix = "pwnwrk-log" })
            });

            container.AddEnvironment("PWNCTL_JobQueue__QueueName", "pwnwrk.fifo");
            container.AddEnvironment("PWNCTL_JobQueue__DLQName", "pwnwrk-dlq.fifo");
            container.AddEnvironment("PWNCTL_JobQueue__IsSQS", "true");
            container.AddEnvironment("PWNCTL_JobQueue__VisibilityTimeout", "30");
            container.AddEnvironment("PWNCTL_Db__ConnectionString", "");
            container.AddEnvironment("PWNCTL_EFS_MOUNT_POINT", "/mnt/efs");

            var volume = new Amazon.CDK.AWS.ECS.Volume
            {
                Name = "pwnwrk-fs",
                EfsVolumeConfiguration = new EfsVolumeConfiguration
                {
                    FileSystemId = fs.FileSystemId,
                    AuthorizationConfig = new AuthorizationConfig
                    {
                        Iam = "DISABLED"
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
                CapacityProviderStrategies = new [] { new CapacityProviderStrategy {
                    CapacityProvider = "FARGATE_SPOT",
                    Weight = 2
                }, new CapacityProviderStrategy {
                    CapacityProvider = "FARGATE",
                    Weight = 1
                } }
            });

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
                        Upper = 80, Change = 2 
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
                        Lower = 80,
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
