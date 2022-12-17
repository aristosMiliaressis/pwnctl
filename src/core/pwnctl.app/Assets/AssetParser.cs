using pwnctl.domain.BaseClasses;
using pwnctl.domain.Entities;
using pwnctl.app.Assets.DTO;
using pwnctl.app.Assets.Exceptions;
using pwnctl.app.Common.Interfaces;

using System.Reflection;

namespace pwnctl.app.Assets
{
    public static class AssetParser
    {
        public static Asset[] Parse(string assetText, out Type[] assetTypes)
        {
            Asset[] assets = null;

            if (string.IsNullOrWhiteSpace(assetText))
                throw new ArgumentException("Null or whitespace asset.", nameof(assetText));

            assetText = assetText.Trim();

            ParseTags(ref assetText, out List<Tag> tags);

            object[] parameters = new object[] { assetText, null, null };
            foreach (var tryParseMethod in _tryParseMethod)
            {
                try
                {
                    bool parsed = (bool)tryParseMethod.Invoke(null, parameters);
                    if (!parsed)
                        continue;

                    var mainAsset = (Asset)parameters[1];
                    tags.ForEach(t => mainAsset[t.Name] = t.Value);

                    assets = (Asset[])parameters[2] ?? new Asset[] {};
                    assets.ToList().ForEach(a => a.FoundBy = mainAsset.FoundBy);
                    assets = assets.Prepend(mainAsset).ToArray();
                }
                catch
                {
                    continue;
                }

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

            var dto = Serializer.Instance.Deserialize<AssetDTO>(assetText);

            assetText = dto.Asset;
            tags = dto.Tags
                    .Where(t => t.Value != null)
                    .Select(t => new Tag(t.Key, t.Value.ToString()))
                    .ToList();
        }

        private static readonly IEnumerable<MethodInfo> _tryParseMethod = Assembly.GetAssembly(typeof(Asset))
                                                .GetTypes()
                                                .Where(t => !t.IsAbstract && typeof(Asset).IsAssignableFrom(t))
                                                .Select(t => t.GetMethod("TryParse"));
    }
}