using pwnwrk.domain.BaseClasses;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;

namespace pwnwrk.infra
{
    public static class CSharpScriptHelper
    {
        /// <summary>
        /// evaluates a CSharpScript expression taking one argument of a type derived from BaseAsset
        /// </summary>
        public static bool Evaluate(string script, BaseAsset asset)
        {
            var options = ScriptOptions.Default.AddReferences(typeof(BaseAsset).Assembly)
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Collections.Generic.IEnumerable<>)));

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