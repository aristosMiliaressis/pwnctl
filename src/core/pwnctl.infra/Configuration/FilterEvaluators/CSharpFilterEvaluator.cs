namespace pwnctl.infra.Configuration;

using pwnctl.domain.BaseClasses;
using pwnctl.app.Common.Interfaces;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Reflection;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Tagging.Entities;

public class CSharpFilterEvaluator : FilterEvaluator
{
    /// <summary>
    /// constructs and evaluates a CSharpScript expression by prefixing the `script` parameter
    /// with a lambda parameter expression `(X, Tags) => ` where X is the name of the concrete type of parameter `asset`
    /// </summary>
    public bool Evaluate(string? filter, AssetRecord record)
    {
        if (string.IsNullOrEmpty(filter))
            return true;

        try
        {
            var funcType = typeof(Func<,,>).MakeGenericType(record.Asset.GetType(), typeof(AssetRecord), typeof(bool));
            var evalMethod = _evaluateAsync.MakeGenericMethod(funcType);

            var evalTask = evalMethod.Invoke(null, new object[]
            {
                "("+record.Asset.GetType().Name+", Tags) => " + filter,
                _scriptOptions, null, null, CancellationToken.None
            });

            var filterExpr = typeof(Task<>).MakeGenericType(funcType).GetProperty("Result").GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<Asset>.Invoke));
            return (bool)invokeMethod.Invoke(filterExpr, new object[] { record.Asset, record });
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    public bool Evaluate(string? filter, Dictionary<string, object> args)
    {
        if (string.IsNullOrEmpty(filter))
            return true;

        try
        {
            var genArgs = args.Values.Select(v => v.GetType()).Append(typeof(bool)).ToArray();

            var funcType = typeof(Func<,,,>).MakeGenericType(genArgs);
            var evalMethod = _evaluateAsync.MakeGenericMethod(funcType);

            var evalTask = evalMethod.Invoke(null, new object[]
            {
                "("+string.Join(",", args.Keys)+") => " + filter,
                _scriptOptions, null, null, CancellationToken.None
            });

            var filterExpr = typeof(Task<>).MakeGenericType(funcType).GetProperty("Result").GetValue(evalTask);
            var invokeMethod = funcType.GetMethod(nameof(Func<Asset>.Invoke));
            return (bool)invokeMethod.Invoke(filterExpr, args.Values.ToArray());
        }
        finally
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

    private static ScriptOptions _scriptOptions = ScriptOptions.Default.AddReferences(typeof(Asset).Assembly)
                                            .AddReferences(typeof(Tag).Assembly)
                                            .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                                            .AddReferences(Assembly.GetAssembly(typeof(System.Collections.Generic.IEnumerable<>)))
                                            .WithImports("System.Collections.Generic", "System.Linq",
                                                        "pwnctl.domain.Enums",
                                                        "pwnctl.domain.Interfaces");
    private static MethodInfo _evaluateAsync = typeof(CSharpScript).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                            .First(m => m.Name == nameof(CSharpScript.EvaluateAsync)
                                                        && m.IsGenericMethod == true);
}