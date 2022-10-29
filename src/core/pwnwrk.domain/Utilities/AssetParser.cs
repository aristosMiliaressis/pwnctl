using pwnwrk.domain.BaseClasses;
using pwnwrk.domain.Entities;
using pwnwrk.domain.Models;
using pwnwrk.domain.Exceptions;
using System.Collections.Generic;
using System.Text.Json;
using System.Reflection;
using System;
using System.Linq;

namespace pwnwrk.domain.Utilities
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

                var assets = (BaseAsset[])parameters[2];
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

            var entry = JsonSerializer.Deserialize<AssetDTO>(assetText, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            assetText = entry.Asset;
            tags = entry.Tags.Select(t => new Tag(t.Key, t.Value.ToString())).ToList();
        }

        private static readonly IEnumerable<MethodInfo> _tryParseMethods = Assembly.GetAssembly(typeof(BaseAsset))
                                                .GetTypes()
                                                .Where(t => !t.IsAbstract && typeof(BaseAsset).IsAssignableFrom(t))
                                                .Select(t => t.GetMethod("TryParse"));
    }
}