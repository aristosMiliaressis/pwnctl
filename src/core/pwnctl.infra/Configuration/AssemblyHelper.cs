namespace pwnctl.infra.Configuration;

using System.Reflection;

public static class AssemblyHelper
{
    public static readonly string AssemblyNamespace = Assembly.GetEntryAssembly().FullName.Split(",")[0];

    //Add COMMIT_HASH assembly attribute at build time
}