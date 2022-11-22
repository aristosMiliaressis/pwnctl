using Amazon.CDK;

using pwnwrk.infra.cdk.Stacks;

namespace pwnwrk.infra.cdk
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