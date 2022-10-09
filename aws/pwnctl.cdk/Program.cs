using Amazon.CDK;

namespace pwnctl.cdk
{
    internal static class Program
    {
        private static void Main()
        {
            var app = new App(new AppProps());

            var pwnctlStack = new PwnctlCdkStack(app, "PwnctlCdkStack", new StackProps());

            Tags.Of(pwnctlStack).Add("stack", "pwnctl");

            app.Synth();
        }
    }
}