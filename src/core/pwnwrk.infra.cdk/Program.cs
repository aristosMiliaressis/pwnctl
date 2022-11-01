using Amazon.CDK;

using pwnwrk.infra.cdk.Stacks;
using pwnwrk.infra.Aws;

namespace pwnwrk.infra.cdk
{
    internal static class Program
    {
        private static void Main()
        {
            var app = new App(new AppProps());

            var pwnctlStack = new PwnctlStack(app, AwsConstants.StackName, new StackProps());

            app.Synth();
        }
    }
}