using Amazon.CDK;

using pwnctl.infra.cdk.Stacks;

namespace pwnctl.infra.cdk
{
    internal static class Program
    {
        private static void Main()
        {
            var app = new App(new AppProps());

            new PwnctlStack(app, "PwnctlStack");

            app.Synth();
        }
    }
}