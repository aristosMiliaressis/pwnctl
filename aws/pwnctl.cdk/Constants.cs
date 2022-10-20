namespace pwnctl.cdk;

internal static class Constants
{
    internal const string VpcId = "pwnwrk-vpc";
    internal const string EfsId = "pwnwrk-fs";
    internal const string EfsMountPoint = "/mnt/efs";
    internal const string EfsApId = "pwnctl-lambda-api-fsap";
    internal const string LambdaName = "pwnctl-api";
    internal const string LambdaRole = "pwnctl-api-role";
    internal const string EcsClusterName = "pwnwrk-cluster";
    internal const string EcsRoleName = "pwnwrk-role";
    internal const string TaskDefinitionId = "pwnwrk-def";
    internal const string ContainerName = "pwnwrk";
    internal const string FargateServiceId = "pwnwrk-svc";
    internal const string QueueName = "pwnwrk.fifo";
    internal const string DLQName = "pwnwrk-dlq.fifo";
    internal const int QueueVisibilityTimeoutInSec = 30;
}