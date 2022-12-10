using pwnctl.domain.BaseClasses;
using pwnctl.app.DTO;
using pwnctl.app.Exceptions;
using pwnctl.app.Interfaces;

using System.Reflection;
using pwnctl.domain.Entities;

namespace pwnctl.app
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

            object[] parameters = new object[] { assetText, null };
            foreach (var parseMethod in _tryParseMethod)
            {
                try
                {
                    bool parsed = (bool)parseMethod.Invoke(null, parameters);
                    if (!parsed)
                        continue;

                    assets = (Asset[])parameters[1];
                    var asset = assets[0];
                    
                    tags.ForEach(t => asset[t.Name] = t.Value);
                    var foundBy = assets.FirstOrDefault(a => a.FoundBy != null)?.FoundBy;
                    assets.ToList().ForEach(a => a.FoundBy = foundBy);
                }
                catch //(Exception ex)
                {
                    //Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
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