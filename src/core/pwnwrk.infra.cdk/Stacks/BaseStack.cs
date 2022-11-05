// using System;
// using System.Text.Json;
// using Amazon.CDK;
// using Amazon.CDK.AWS.EC2;
// using Amazon.CDK.AWS.EFS;
// using Amazon.CDK.AWS.SQS;
// using Amazon.CDK.AWS.RDS;
// using Amazon.CDK.AWS.Logs;
// using Amazon.CDK.AWS.SecretsManager;
// using SecretsManager = Amazon.CDK.AWS.SecretsManager;
// using EFS = Amazon.CDK.AWS.EFS;
// using RDS = Amazon.CDK.AWS.RDS;
// using pwnwrk.infra.Aws;

// namespace pwnwrk.infra.cdk.Stacks
// {
//     internal sealed class BaseStack : Stack
//     {
//         internal static Vpc Vpc { get; set; }
//         internal static Queue Queue { get; set; }
//         internal static Queue DLQueue { get; set; }
//         internal static EFS.FileSystem FileSystem { get; set; }
//         internal static DatabaseCluster DbCluster { get; set; }
//         internal static SecretsManager.Secret DbSecret { get; set; }
//         internal static string UsafeDbPassword { get; set; }
//         internal static bool EnableLambdaSecurityManager { get; set; }

//         internal BaseStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
//         {
//             // Enabling SecretsManager on the lambda requires a VPC endpoint(cause the lamda is in a private subnet) which costs 7.20$ per month
//             var lambdaSecMngrParam = new Amazon.CDK.CfnParameter(this, "lambdaSecMngrEp", new Amazon.CDK.CfnParameterProps
//             {
//                 Type = "String",
//                 Description = "Flag indicating wether to use secret manager oe env vars for the lambda api"
//             });

//             EnableLambdaSecurityManager = lambdaSecMngrParam.ValueAsString.ToLower().StartsWith("y");
            
//             #region VPC
//             Vpc = new Vpc(this, AwsConstants.VpcId, new VpcProps
//             {
//                 MaxAzs = 2,
//                 NatGateways = 0,
//                 SubnetConfiguration = new SubnetConfiguration[]
//                 {
//                     new SubnetConfiguration
//                     {
//                         Name = AwsConstants.PublicSubnet1,
//                         SubnetType = SubnetType.PUBLIC,
//                         CidrMask = 24
//                     },
//                     new SubnetConfiguration
//                     {
//                         Name = AwsConstants.PrivateSubnet1,
//                         SubnetType = SubnetType.PRIVATE_ISOLATED,
//                         CidrMask = 24
//                     }
//                 }
//             });
//             #endregion

//             #region Aurora RDS
//             (DbSecret, UsafeDbPassword) = CreateDbSecret(EnableLambdaSecurityManager);

//             var dbEngine = DatabaseClusterEngine.AuroraPostgres(new AuroraPostgresClusterEngineProps
//             {
//                 Version = AuroraPostgresEngineVersion.VER_13_6
//             });

//             DbCluster = new DatabaseCluster(this, AwsConstants.AuroraCluster, new DatabaseClusterProps
//             {
//                 ClusterIdentifier = AwsConstants.AuroraCluster,
//                 Engine = dbEngine,
//                 Credentials = Credentials.FromSecret(DbSecret),
//                 InstanceProps = new RDS.InstanceProps
//                 {
//                     InstanceType = InstanceType.Of(InstanceClass.BURSTABLE3, InstanceSize.MEDIUM),
//                     VpcSubnets = new SubnetSelection
//                     {
//                         SubnetType = SubnetType.PRIVATE_ISOLATED
//                     },
//                     Vpc = Vpc,
//                 },
//                 RemovalPolicy = RemovalPolicy.DESTROY,
//                 Instances = 1,
//                 InstanceIdentifierBase = AwsConstants.AuroraInstance,
//                 CloudwatchLogsRetention = RetentionDays.ONE_WEEK,
//                 DefaultDatabaseName = AwsConstants.DatabaseName,
//             });
//             #endregion

//             #region SQS
//             DLQueue = new Queue(this, AwsConstants.DLQName, new QueueProps
//             {
//                 QueueName = AwsConstants.DLQName,
//                 Encryption = QueueEncryption.UNENCRYPTED,
//                 ContentBasedDeduplication = true,
//                 MaxMessageSizeBytes = 8192,
//                 ReceiveMessageWaitTime = Duration.Seconds(20),
//                 RetentionPeriod = Duration.Days(14),
//                 VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec)
//             });

//             Queue = new Queue(this, AwsConstants.QueueName, new QueueProps
//             {
//                 QueueName = AwsConstants.QueueName,
//                 Encryption = QueueEncryption.UNENCRYPTED,
//                 ContentBasedDeduplication = true,
//                 MaxMessageSizeBytes = 8192,
//                 ReceiveMessageWaitTime = Duration.Seconds(20),
//                 RetentionPeriod = Duration.Days(1),
//                 VisibilityTimeout = Duration.Seconds(AwsConstants.QueueVisibilityTimeoutInSec),
//                 DeadLetterQueue = new DeadLetterQueue
//                 {
//                     MaxReceiveCount = 10,
//                     Queue = DLQueue
//                 }
//             });
//             #endregion

//             #region EFS
//             FileSystem = new EFS.FileSystem(this, AwsConstants.EfsId, new FileSystemProps
//             {
//                 Vpc = Vpc,
//                 RemovalPolicy = RemovalPolicy.RETAIN
//             });

//             // new CfnOutput(this, "myBucketRef", new CfnOutputProps{
//             //     Value = FileSystem,
//             //     Description = "The name of the s3 bucket",
//             //     ExportName = "myBucket",
//             // });
//             #endregion
//         }
        
//         public (SecretsManager.Secret, string) CreateDbSecret(bool lambdaSecMngrEnabled)
//         {
//             if (lambdaSecMngrEnabled)
//             {
//                 var dbSecret = new SecretsManager.Secret(this, AwsConstants.DatabaseCredSecret, new SecretProps
//                 {
//                     SecretName = AwsConstants.DatabaseCredSecretName,
//                     GenerateSecretString = new SecretStringGenerator
//                     {
//                         SecretStringTemplate = JsonSerializer.Serialize(new { username = AwsConstants.AuroraInstanceUsername }),
//                         ExcludePunctuation = true,
//                         IncludeSpace = false,
//                         GenerateStringKey = "password"
//                     }
//                 });

//                 return (dbSecret, null);
//             }

//             var unsafeDbPassword = Guid.NewGuid().ToString();
//             // new CfnOutput(this, "UnsafeDbPasswordOutput", new CfnOutputProps
//             // {
//             //     Value = unsafeDbPassword,
//             //     Description = "Unsafe Db Password Output",
//             //     ExportName = "UnsafeDbPasswordOutput",
//             // });

//             var dbCreds = new
//             {
//                 username = AwsConstants.AuroraInstanceUsername,
//                 password = unsafeDbPassword,
//             };

//             var unsafeDbSecret = new SecretsManager.Secret(this, AwsConstants.DatabaseCredSecret, new SecretProps
//             {
//                 SecretName = AwsConstants.DatabaseCredSecretName,
//                 SecretStringValue = SecretValue.UnsafePlainText(JsonSerializer.Serialize(dbCreds)),
//             });

//             return (unsafeDbSecret, unsafeDbPassword);
//         }
//     }
// }
