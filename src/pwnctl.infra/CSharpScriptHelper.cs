using pwnctl.core.BaseClasses;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.CSharp;

namespace pwnctl.infra
{
    // this is a workaround to use `CSharpScript` with single-file publish
    // stolen from: https://github.com/andersstorhaug/SingleFileScripting
    public static class CSharpScriptHelper
    {
        public static bool Evaluate(string script, BaseAsset asset)
        {
            var references = new List<MetadataReference>
            {
                GetReference(typeof(BaseAsset)),
                GetReference(typeof(System.Linq.Enumerable)),
                GetReference(typeof(System.Collections.Generic.IEnumerable<>))
            };

            Assembly.GetEntryAssembly().GetReferencedAssemblies()
                .ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            var syntaxTree = SyntaxFactory.ParseSyntaxTree(
                $"{asset.GetType().Name} => " + script,
                new CSharpParseOptions(
                    kind: SourceCodeKind.Script,
                    languageVersion: LanguageVersion.Latest));

            var funcType = typeof(Func<,>).MakeGenericType(asset.GetType(), typeof(bool));

            var compilation = CSharpCompilation.CreateScriptCompilation(
                assemblyName: "Script",
                syntaxTree,
                references,
                returnType: funcType);

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(object[]), typeof(Task<>).MakeGenericType(funcType));
            object submissionFactory = null;

            using (var peStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                // https://github.com/dotnet/roslyn/blob/version-3.2.0/src/Scripting/Core/ScriptBuilder.cs#L121
                // https://github.com/dotnet/roslyn/blob/version-3.2.0/src/Scripting/Core/Utilities/PdbHelpers.cs#L10
                var result = compilation.Emit(
                    peStream,
                    pdbStream,
                    xmlDocumentationStream: null,
                    win32Resources: null,
                    manifestResources: null,
                    new EmitOptions(
                        debugInformationFormat: DebugInformationFormat.PortablePdb,
                        pdbChecksumAlgorithm: default(HashAlgorithmName)));

                if (!result.Success)
                {
                    throw new Exception($"Compilation of CScript '{script}' failed");
                }

                var scriptAssembly = AppDomain.CurrentDomain.Load(peStream.ToArray(), pdbStream.ToArray());

                // https://github.com/dotnet/roslyn/blob/version-3.2.0/src/Scripting/Core/ScriptBuilder.cs#L188
                var entryPoint = compilation.GetEntryPoint(CancellationToken.None) ?? throw new InvalidOperationException("Entry point could be determined");

                var entryPointType = scriptAssembly
                    .GetType(
                        $"{entryPoint.ContainingNamespace.MetadataName}.{entryPoint.ContainingType.MetadataName}",
                        throwOnError: true,
                        ignoreCase: false);

                var entryPointMethod = entryPointType?.GetTypeInfo().GetDeclaredMethod(entryPoint.MetadataName) ?? throw new InvalidOperationException("Entry point method could be determined");

                var createDelegateMethod = typeof(MethodInfo)
                                                .GetMethod(nameof(MethodInfo.CreateDelegate), new Type[] { })
                                                .MakeGenericMethod(delegateType);
                submissionFactory = createDelegateMethod.Invoke(entryPointMethod, new object[] { });
            }

            var delegateInvokeMethod = delegateType.GetMethod(nameof(Func<BaseAsset>.Invoke));

            var evalTask = delegateInvokeMethod.Invoke(submissionFactory, new object[] { new object[] { null, null } });

            var filter = typeof(Task<>).MakeGenericType(funcType).GetProperty(nameof(Task<BaseAsset>.Result)).GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<BaseAsset>.Invoke));
            return (bool)invokeMethod.Invoke(filter, new object[] { asset });
        }

        // https://github.com/dotnet/runtime/issues/36590#issuecomment-689883856
        private static MetadataReference GetReference(Type type)
        {
            unsafe
            {
                return type.Assembly.TryGetRawMetadata(out var blob, out var length)
                    ? AssemblyMetadata
                        .Create(ModuleMetadata.CreateFromMetadata((IntPtr)blob, length))
                        .GetReference()
                    : throw new InvalidOperationException($"Could not get raw metadata for type {type}");
            }
        }
    }
}