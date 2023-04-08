using pwnctl.domain.BaseClasses;
using pwnctl.app.Common.Interfaces;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tagging.Entities;

namespace pwnctl.infra.Configuration
{
    public class CSharpFilterEvaluator : FilterEvaluator
    {
        private static ScriptOptions _scriptOptions = ScriptOptions.Default.AddReferences(typeof(Asset).Assembly)
                                                .AddReferences(typeof(Tag).Assembly)
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                                                .AddReferences(Assembly.GetAssembly(typeof(System.Collections.Generic.IEnumerable<>)))
                                                .WithImports("System.Collections.Generic",
                                                            "pwnctl.domain.Enums",
                                                            "pwnctl.domain.Interfaces");
        private static MethodInfo _evaluateAsync = typeof(CSharpScript).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                .First(m => m.Name == nameof(CSharpScript.EvaluateAsync)
                                                         && m.IsGenericMethod == true);
        /// <summary>
        /// constructs and evaluates a CSharpScript expression by prefixing the `script` parameter
        /// with a lambda parameter expression `(X, Tags) => ` where X is the name of the concrete type of parameter `asset`
        /// </summary>
        public bool Evaluate(string script, AssetRecord record)
        {
            try
            {
                var funcType = typeof(Func<,,>).MakeGenericType(record.Asset.GetType(), typeof(AssetRecord), typeof(bool));
                var evalMethod = _evaluateAsync.MakeGenericMethod(funcType);

                var evalTask = evalMethod.Invoke(null, new object[]
                {
                    "("+record.Asset.GetType().Name+", Tags) => " + script,
                    _scriptOptions, null, null, CancellationToken.None
                });

                var filter = typeof(Task<>).MakeGenericType(funcType).GetProperty("Result").GetValue(evalTask);
                var invokeMethod = funcType.GetMethod(nameof(Func<Asset>.Invoke));
                return (bool)invokeMethod.Invoke(filter, new object[] { record.Asset, record });
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }
}