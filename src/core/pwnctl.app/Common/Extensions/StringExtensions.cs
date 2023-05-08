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
                throw new InvalidTemplateStringException($"Property {param} not found on type {sourceType.Name}");

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

    public static object Extrapolate(this string input, string template, object dest)
    {
        var destType = dest.GetType();

        foreach (var seg in template.Split("}"))
        {
            if (!seg.Contains("{"))
                continue;

            input = input.Substring(seg.Split("{")[0].Length);

            var param = seg.Split("{")[1];
            var arg = input.Split("/")[0];

            var prop = destType.GetProperty(param);
            if (prop == null)
                throw new InvalidTemplateStringException($"Property {param} not found on type {destType.Name}");

            prop.SetValue(dest, arg);
        }

        return dest;
    }

    public static bool ValidateTemplate(this string template, Type objType)
    {
        List<string> arguments = new();
        List<string> parameters = new();

        foreach (var seg in template.Split("}"))
        {
            if (!seg.Contains("{"))
                continue;

            parameters.Add(seg.Split("{")[1]);
        }

        foreach (var param in parameters)
        {
            var prop = objType.GetProperty(param);
            if (prop == null)
                throw new InvalidTemplateStringException($"Property {param} not found on type {objType.Name}");
        }

        return true;
    }
}
