using pwnctl.core.BaseClasses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using System.Reflection.Metadata;

namespace pwnctl.infra
{
    public static class CSharpScriptHelper
    {
        // this is a workaround to use `CSharpScript` with single-file publish
        // stolen from: https://github.com/andersstorhaug/SingleFileScripting
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


        public static bool Evaluate(string script, BaseAsset asset)
        {
            // var options = ScriptOptions.Default.AddReferences(typeof(BaseAsset).Assembly)
            //                                     .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
            //                                     .AddReferences(Assembly.GetAssembly(typeof(System.Collections.Generic.IEnumerable<>)));


            var references = new[]
            {
                GetReference(typeof(BaseAsset)),
                GetReference(typeof(System.Linq.Enumerable)),
                GetReference(typeof(System.Collections.Generic.IEnumerable<>))
            };

            var options = ScriptOptions.Default.AddReferences(references);

            var funcType = typeof(Func<,>).MakeGenericType(asset.GetType(), typeof(bool));
            var evalMethod = typeof(CSharpScript).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                .First(m => m.Name == nameof(CSharpScript.EvaluateAsync)
                                                         && m.IsGenericMethod == true)
                                                .MakeGenericMethod(funcType);
            var evalTask = evalMethod.Invoke(null, new object[]
            {
                    $"{asset.GetType().Name} => " + script,
                    options, null, null, CancellationToken.None
            });

            var filter = typeof(Task<>).MakeGenericType(funcType).GetProperty(nameof(Task<BaseAsset>.Result)).GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<BaseAsset>.Invoke));
            return (bool)invokeMethod.Invoke(filter, new object[] { asset });
        }
    }
}