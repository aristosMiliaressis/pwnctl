namespace pwnwrk.infra.Aws;

public static class AwsConstants
{
    public const string StackName = "pwnctl";
    public const string VpcId = "pwnwrk-vpc";
    public const string VpcSecMgrEndpoint = "pwnctl-lambda-sec-mgr-ep";
    public const string AuroraCluster = $"{StackName}-aurora-cluster";
    public const string AuroraInstance = $"{StackName}-aurora-postgresql-instance";
    public const string AuroraInstanceUsername = "pwnadmin";
    public const string DatabaseName = StackName;
    public const string QueueName = "pwnwrk.fifo";
    public const string DLQName = "pwnwrk-dlq.fifo";
    public const int QueueVisibilityTimeoutInSec = 30;
    public const string EfsId = "pwnwrk-fs";
    public const string EfsMountPoint = "/mnt/efs";
    public const string EfsApId = $"{StackName}-lambda-api-fsap";
    public const string EcsClusterName = "pwnwrk-cluster";
    public const string EcsLogGroupId = "pwnwrk-logs";
    public const string EcsLogGroupName = "/aws/ecs/pwnwrk";
    public const string EcsRoleName = "pwnwrk-role";
    public const string TaskDefinitionId = "pwnwrk-def";
    public const string ContainerName = "pwnwrk";
    public const string FargateServiceId = "pwnwrk-svc";
    public const string LambdaName = $"{StackName}-api";
    public const string LambdaRole = $"{StackName}-api-role";
    public const string ApiKeyName = $"{StackName}-Api-ApiKey";
}