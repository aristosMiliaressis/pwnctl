namespace pwnctl.infra.Aws;

public static class AwsConstants
{
    public const string VpcId = "PwnctlVpc";
    public const string PublicSubnet1 = "PublicSubnet1";
    public const string PrivateSubnet1 = "PrivateSubnet1";
    public const string AuroraCluster = "PwnctlAuroraCluster";
    public const string AuroraInstance = "PwnctlAuroraPostgreSqlInstance";
    public const string AuroraInstanceUsername = "pwnadmin";
    public const string DatabaseName = "pwnctl";
    public const string DatabaseCredSecret = $"{AuroraInstance}Creds";
    public const string DatabaseCredSecretName = $"/aws/secret/pwnctl/Db";
    public const string QueueName = "pwnctl.fifo";
    public const string DLQName = "pwnctl-dlq.fifo";
    public const double EcsInstances = 20;
    public const int MaxTaskTimeout = 5400;
    public const int QueueVisibilityTimeoutInSec = 600;
    public const string EfsId = "PwnctlFs";
    public const string EfsMountPoint = "/mnt/efs";
    public const string EcsClusterName = "PwnctlCluster";
    public const string EcsLogGroupId = "PwnctlWorkerLogs";
    public const string EcsRoleName = "PwnctlRole";
    public const string TaskDefinitionId = "PwnctlDef";
    public const string ContainerName = "pwnctl";
    public const string FargateServiceId = "PwnctlSvc";
    public const string LambdaName = "PwnctlApi";
    public const string LambdaRole = $"{LambdaName}Role";
    public const string EfsApId = $"{LambdaName}Fsap";
    public const string LambdaLogGroup = $"/aws/lambda/{LambdaName}";
    public const string EcsLogGroupName = "/aws/ecs/pwnctl";
    public const string ApiUrlParamId = "PwnctlApiUrlParam";
    public const string ApiUrlParam = "/pwnctl/Api/BaseUrl";
    public const string ScaleOutPolicy = "ScaleOutPolicy";
}