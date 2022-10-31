using Amazon.CDK;

using pwnwrk.infra.Aws;

namespace pwnctl.cdk
{
    internal static class Program
    {
        private static void Main()
        {
            var app = new App(new AppProps());

            var pwnctlStack = new PwnctlCdkStack(app, AwsConstants.StackName, new StackProps());

            app.Synth();
        }
    }
}