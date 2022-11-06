// using System;
// using System.IO;
// using System.Linq;
// // using System.Collections.Generic;
// using Amazon.CDK;
// using Amazon.CDK.AWS.EC2;
// using Amazon.CDK.AWS.ECS;
// using Amazon.CDK.AWS.EFS;
// using Amazon.CDK.AWS.SQS;
// using Amazon.CDK.AWS.IAM;
// using Amazon.CDK.AWS.RDS;
// using Amazon.CDK.AWS.SSM;
// using Amazon.CDK.AWS.Lambda;
// using Amazon.CDK.AWS.CloudWatch;
// using Amazon.CDK.AWS.ApplicationAutoScaling;
// using Amazon.CDK.AWS.Logs;
// using Amazon.CDK.AWS.SecretsManager;
// using SecretsManager = Amazon.CDK.AWS.SecretsManager;
// using EFS = Amazon.CDK.AWS.EFS;
// using ECS = Amazon.CDK.AWS.ECS;
// using RDS = Amazon.CDK.AWS.RDS;
// using Lambda = Amazon.CDK.AWS.Lambda;
// using pwnwrk.infra.Aws;
// using pwnwrk.infra.Logging;

// namespace pwnwrk.infra.cdk.Stacks
// {
//     internal sealed class PwnctlStack : Stack
//     {
//         internal PwnctlStack(Construct scope, string id, IStackProps props = null)
//             : base(scope, id, props)
//         {
//             var (apiKeySecret, unsafeApiKey) = CreateApiSecret(BaseStack.EnableLambdaSecurityManager);

//             var pwnctlApiRole = new Role(this, AwsConstants.LambdaRole, new RoleProps
//             {
//                 AssumedBy = new ServicePrincipal("lambda.amazonaws.com")
//             });

//             pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaBasicExecutionRole"));
//             pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("service-role/AWSLambdaVPCAccessExecutionRole"));
//             pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonElasticFileSystemClientFullAccess"));
//             pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("AmazonRDSFullAccess"));
//             BaseStack.Queue.GrantSendMessages(pwnctlApiRole);

//             // Costs 7,20$ per month
//             var sqsEp = new InterfaceVpcEndpoint(this, AwsConstants.VpcSQSEndpoint, new InterfaceVpcEndpointProps
//             {
//                 Vpc = BaseStack.Vpc,
//                 Service = InterfaceVpcEndpointAwsService.SQS,
//                 Subnets = new SubnetSelection
//                 {
//                     Subnets = BaseStack.Vpc.IsolatedSubnets
//                 }
//             });

//             var accessPoint = BaseStack.FileSystem.AddAccessPoint(AwsConstants.EfsApId, new AccessPointOptions
//             {
//                 CreateAcl = new Acl { OwnerGid = "1001", OwnerUid = "1001", Permissions = "777" },
//                 PosixUser = new PosixUser { Gid = "0", Uid = "0" },
//                 Path = "/"
//             });

//             new CfnOutput(this, AwsConstants.PwnctlEfsAccessPointId, new CfnOutputProps{
//                 Value = accessPoint.AccessPointId,
//                 Description = "The EFS access point id",
//                 ExportName = AwsConstants.PwnctlEfsAccessPointId,
//             });

//             var function = new Function(this, AwsConstants.LambdaName, new FunctionProps
//             {
//                 Runtime = Runtime.DOTNET_6,
//                 MemorySize = 512,
//                 Timeout = Duration.Seconds(180),
//                 Code = Code.FromAsset(Path.Join("src", "pwnctl", "pwnctl.api", "bin", "Release", "net6.0")),
//                 Handler = "pwnctl.api",
//                 Vpc = BaseStack.Vpc,
//                 Role = pwnctlApiRole,
//                 Filesystem = Lambda.FileSystem.FromEfsAccessPoint(accessPoint, AwsConstants.EfsMountPoint),
//                 LogRetention = RetentionDays.ONE_WEEK,
//                 Environment = new Dictionary<string, string>()
//                 {
//                     {"PWNCTL_Aws__InVpc", "true"},
//                     {"PWNCTL_JobQueue__QueueName", AwsConstants.QueueName},
//                     {"PWNCTL_JobQueue__DLQName", AwsConstants.DLQName},
//                     {"PWNCTL_JobQueue__VisibilityTimeout", AwsConstants.QueueVisibilityTimeoutInSec.ToString()},
//                     {"PWNCTL_Logging__Provider", LogProfile.Console.ToString()},
//                     {"PWNCTL_Logging__MinLevel", "Debug"},
//                     {"PWNCTL_Logging__LogGroup", AwsConstants.LambdaLogGroup},
//                     {"PWNCTL_InstallPath", AwsConstants.EfsMountPoint}
//                 }
//             });
//             BaseStack.DbCluster.Connections.AllowDefaultPortFrom(function);

//             var fnUrl = function.AddFunctionUrl(new FunctionUrlOptions
//             {
//                 AuthType = FunctionUrlAuthType.NONE
//             });

//             new StringParameter(this, AwsConstants.ApiUrlParamId, new StringParameterProps
//             {
//                 ParameterName = AwsConstants.ApiUrlParam,
//                 StringValue = fnUrl.Url,
//                 Description = $"the base url of {AwsConstants.LambdaName}"
//             });

//             if (BaseStack.EnableLambdaSecurityManager)
//             {
//                 pwnctlApiRole.AddManagedPolicy(ManagedPolicy.FromAwsManagedPolicyName("SecretsManagerReadWrite"));
//                 apiKeySecret.GrantRead(pwnctlApiRole);
//                 BaseStack.DbSecret.GrantRead(pwnctlApiRole);

//                 // Costs 7.20$ per month
//                 var secMgrEp = new InterfaceVpcEndpoint(this, AwsConstants.VpcSecMgrEndpoint, new InterfaceVpcEndpointProps
//                 {
//                     Vpc = BaseStack.Vpc,
//                     Service = InterfaceVpcEndpointAwsService.SECRETS_MANAGER,
//                     Subnets = new SubnetSelection
//                     {
//                         Subnets = BaseStack.Vpc.IsolatedSubnets
//                     }
//                 });

//                 return;
//             }

//             function.AddEnvironment("PWNCTL_Db__Host", BaseStack.DbCluster.ClusterEndpoint.Hostname);
//             function.AddEnvironment("PWNCTL_Db__Password", BaseStack.UsafeDbPassword);
//             function.AddEnvironment("PWNCTL_Api__ApiKey", unsafeApiKey);
//         }

//         public (SecretsManager.Secret, string) CreateApiSecret(bool lambdaSecMngrEnabled)
//         {
//             if (lambdaSecMngrEnabled)
//             {
//                 var apiSecret = new SecretsManager.Secret(this, AwsConstants.ApiKeyId, new SecretProps
//                 {
//                     SecretName = AwsConstants.ApiKeyName,
//                     GenerateSecretString = new SecretStringGenerator
//                     {
//                         ExcludePunctuation = true,
//                         IncludeSpace = false
//                     }
//                 });

//                 return (apiSecret, null);
//             }

//             var unsafeApiKey = Guid.NewGuid().ToString();

//             var unsafeApiSecret = new SecretsManager.Secret(this, AwsConstants.ApiKeyId, new SecretProps
//             {
//                 SecretName = AwsConstants.ApiKeyName,
//                 SecretStringValue = SecretValue.UnsafePlainText(unsafeApiKey),
//             });

//             return (unsafeApiSecret, unsafeApiKey);
//         }
//     }
// }
