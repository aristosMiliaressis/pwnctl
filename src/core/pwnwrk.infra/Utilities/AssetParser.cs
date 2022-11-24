using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.Entities;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.infra.Exceptions;
using pwnwrk.infra;
using System.Reflection;

namespace pwnwrk.infra.Utilities
{
    public static class AssetParser
    {
        public static bool TryParse(string assetText, out Type[] assetTypes, out Asset[] assets)
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

        public static Asset[] Parse(string assetText, out Type[] assetTypes)
        {
            if (string.IsNullOrWhiteSpace(assetText))
                throw new ArgumentException("Null or whitespace asset.", nameof(assetText));

            assetText = assetText.Trim();

            ParseTags(ref assetText, out List<Tag> tags);

            object[] parameters = new object[] { assetText, tags, null };
            foreach (var tryParseMethod in _tryParseMethods)
            {
                try
                {
                    bool parsed = (bool)tryParseMethod.Invoke(null, parameters);
                    if (!parsed)
                        continue;
                }
                catch
                {
                    continue;
                }

                var assets = (Asset[])parameters[2];
                var foundBy = assets.FirstOrDefault(a => a.FoundBy != null)?.FoundBy;
                assets.ToList().ForEach(a => a.FoundBy = foundBy);
                assetTypes = assets.Select(a => a.GetType()).ToArray();
                return assets;
            }

            throw new UnparsableAssetException(assetText);
        }

        private static void ParseTags(ref string assetText, out List<Tag> tags)
        {
            tags = new();

            // raw non json input doesn't support tags
            if (!assetText.StartsWith("{"))
                return;

            var entry = PwnContext.Serializer.Deserialize<AssetDTO>(assetText);

            assetText = entry.Asset;
            tags = entry.Tags
                    .Where(t => t.Value != null)
                    .Select(t => new Tag(t.Key, t.Value.ToString()))
                    .ToList();
        }

        private static readonly IEnumerable<MethodInfo> _tryParseMethods = Assembly.GetAssembly(typeof(Asset))
                                                .GetTypes()
                                                .Where(t => !t.IsAbstract && typeof(Asset).IsAssignableFrom(t))
                                                .Select(t => t.GetMethod("TryParse"));
    }
}