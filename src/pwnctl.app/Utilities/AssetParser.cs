using pwnctl.core.BaseClasses;
using pwnctl.core.Entities;
using pwnctl.app.Exceptions;
using pwnctl.infra.Configuration;
using pwnctl.infra.Logging;
using System.Reflection;

namespace pwnctl.app.Utilities
{
    public static class AssetParser
    {
        public static bool TryParse(string assetText, out Type[] assetTypes, out BaseAsset[] assets)
        {
            try
            {
                assets = Parse(assetText, out assetTypes);
                return true;
            }
            catch
            {
                assetTypes = null;
                assets = null;
                return false;
            }
        }

        public static BaseAsset[] Parse(string assetText, out Type[] assetTypes)
        {
            if (string.IsNullOrWhiteSpace(assetText))
                throw new ArgumentException("Null or whitespace asset.", nameof(assetText));

            ParseTags(ref assetText, out List<Tag> tags);

            object[] parameters = new object[] { assetText, tags, null };
            foreach (var tryParseMethod in _tryParseMethods)
            {
                bool parsed = false;
                try
                {
                    parsed = (bool)tryParseMethod.Invoke(null, parameters);
                }
                catch
                {
                    continue;
                }

                if (parsed)
                {
                    var assets = (BaseAsset[])parameters[2];
                    assetTypes = assets.Select(a => a.GetType()).ToArray();
                    return assets;
                }
            }

            throw new UnrecognizableAssetException(assetText);
        }

        private static void ParseTags(ref string assetText, out List<Tag> tags)
        {
            tags = new();
            var parts = assetText.Split(EnvironmentVariables.PWNCTL_DELIMITER);
            assetText = parts[0];
            if (parts.Length > 1)
            {
                tags = parts.Skip(1).Select(p => new Tag(p.Split(":")[0], p.Split(":")[1])).ToList();
            }
        }

        private static readonly IEnumerable<MethodInfo> _tryParseMethods = Assembly.GetAssembly(typeof(BaseAsset))
                                                .GetTypes()
                                                .Where(t => !t.IsAbstract && typeof(BaseAsset).IsAssignableFrom(t))
                                                .Select(t => t.GetMethod("TryParse"));
    }
}