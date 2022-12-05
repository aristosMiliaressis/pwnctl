using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Interfaces;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;

namespace pwnwrk.infra.Configuration
{
    public class CSharpFilterEvaluator : FilterEvaluator
    {
        /// <summary>
        /// constructs and evaluates a CSharpScript expression by prefixing the `script` parameter
        /// with a lambda parameter expression `X => ` where X is the name of the concrete type of parameter `asset`
        /// </summary>
        public bool Evaluate(string script, Asset asset)
        {
            var options = ScriptOptions.Default.AddReferences(typeof(Asset).Assembly)
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

            var filter = typeof(Task<>).MakeGenericType(funcType).GetProperty(nameof(Task<Asset>.Result)).GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<Asset>.Invoke));
            return (bool)invokeMethod.Invoke(filter, new object[] { asset });
        }
    }
}