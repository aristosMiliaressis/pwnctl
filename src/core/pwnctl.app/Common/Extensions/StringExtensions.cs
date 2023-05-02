using pwnctl.app.Tasks.Exceptions;

namespace pwnctl.app.Common.Extensions;

public static class StringExtensions
{
    public static string Interpolate(this string template, object source)
    {
        List<string> arguments = new();
        List<string> parameters = new();

        foreach (var seg in template.Split("}}"))
        {
            if (!seg.Contains("{{"))
                continue;

            parameters.Add(seg.Split("{{")[1]);
        }

        var sourceType = source.GetType();

        foreach (var param in parameters)
        {
            var prop = sourceType.GetProperty(param);
            if (prop == null)
                throw new StringInterpolationException($"Property {param} not found on type {sourceType.Name}");

            var arg = prop.GetValue(source);

            arguments.Add(arg.ToString());
        }

        string result = template;

        foreach (var arg in arguments.Distinct())
        {
            result = result.Replace("{{" + result.Split("{{")[1].Split("}}")[0] + "}}", arg);
        }

        return result;
    }
}
