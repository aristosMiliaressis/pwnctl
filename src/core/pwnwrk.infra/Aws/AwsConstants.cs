namespace pwnwrk.infra.Aws;

public static class AwsConstants
{
    public const string StackName = "PwnctlStack";
    public const string VpcId = "PwnwrkVpc";
    public const string PublicSubnet1 = "PublicSubnet1";
    public const string PrivateSubnet1 = "PrivateSubnet1";
    public const string VpcSecMgrEndpoint = "PwnctlLambdaSecMgrEp";
    public const string AuroraCluster = "PwnctlAuroraCluster";
    public const string AuroraInstance = "PwnctlAuroraPostgreSqlInstance";
    public const string AuroraInstanceUsername = "pwnadmin";
    public const string DatabaseName = "pwnctl";
    public const string DatabaseCredSecret = $"{AuroraInstance}Creds";
    public const string QueueName = "pwnwrk.fifo";
    public const string DLQName = "pwnwrk-dlq.fifo";
    public const int QueueVisibilityTimeoutInSec = 30;
    public const string EfsId = "PwnwrkFs";
    public const string EfsMountPoint = "/mnt/efs";
    public const string EcsClusterName = "PwnwrkCluster";
    public const string EcsLogGroupId = "PwnwrkLogs";
    public const string EcsRoleName = "PwnwrkRole";
    public const string TaskDefinitionId = "PwnwrkDef";
    public const string ContainerName = "pwnwrk";
    public const string FargateServiceId = "PwnwrkSvc";
    public const string LambdaName = "PwnctlApi";
    public const string LambdaRole = $"{LambdaName}Role";
    public const string EfsApId = $"{LambdaName}Fsap";
    public const string LambdaLogGroup = $"/aws/lambda/{LambdaName}";
    public const string EcsLogGroupName = "/aws/ecs/pwnwrk";
    public const string DatabaseCredSecretName = $"/aws/secret/pwnctl/Db";
    public const string ApiKeyId = "PwnctlApiKeySecret";
    public const string ApiKeyName = "/aws/secret/pwnctl/Api/ApiKey";
    public const string ApiUrlParamId = "PwnctlApiUrlParam";
    public const string ApiUrlParam = "/pwnctl/Api/BaseUrl";
    public const string ScaleInPolicy = "ScaleInPolicy";
    public const string ScaleOutPolicy = "ScaleOutPolicy";
}