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

            // var baseStack = new BaseStack(app, "PwnBaseStack");

            // var pwnctlStack = new PwnctlStack(app, "PwnctlStack");

            // var pwnwrkStack = new PwnwrkStack(app, "PwnwrkStack");

            new PwnctlMonolithStack(app, "PwnctlStack");

            app.Synth();
        }
    }
}