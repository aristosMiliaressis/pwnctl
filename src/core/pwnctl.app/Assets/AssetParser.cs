namespace pwnctl.app.Assets;

using pwnctl.kernel.BaseClasses;
using pwnctl.kernel.Extensions;
using pwnctl.domain.BaseClasses;
using System.Reflection;

public static class AssetParser
{
    public static Result<Asset, string> Parse(string? assetText)
    {
        if (string.IsNullOrEmpty(assetText))
            return $"Argument '{nameof(assetText)}' was null or empty";

        assetText = assetText.Trim();

        foreach (var tryParseMethod in _tryParseMethod)
        {
            // TODO: clean this up
            Type resultType = typeof(Result<,>).MakeGenericType(tryParseMethod.ReturnType.GenericTypeArguments[0], typeof(string));
            PropertyInfo Result_IsOk_Property = resultType.GetProperty("IsOk");
            FieldInfo Result_Value_Property = resultType.GetField("Value");

            try
            {
                var result = tryParseMethod?.Invoke(null, new object[] { assetText });
                if (!(bool)Result_IsOk_Property.GetValue(result))
                    continue;

                return (Asset)Result_Value_Property.GetValue(result);
            }
            catch
            {
                continue;
            }
        }

        return "Can't parse asset " + assetText;
    }

    private static readonly IEnumerable<MethodInfo?> _tryParseMethod = Assembly.GetAssembly(typeof(Asset))
                !.GetTypes()
                .Where(t => !t.IsAbstract && typeof(Asset).IsAssignableFrom(t))
                .Select(t => t.GetMethod("TryParse"));

    
}