using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Exceptions;

using System.Reflection;

namespace pwnctl.app.Assets
{
    public static class AssetParser
    {
        public static Asset Parse(string? assetText)
        {
            if (string.IsNullOrEmpty(assetText))
                throw new ArgumentException(assetText, nameof(assetText));

            assetText = assetText.Trim();

            foreach (var tryParseMethod in _tryParseMethod)
            {
                try
                {
                    Asset? asset = (Asset?)tryParseMethod?.Invoke(null, new object[] { assetText });
                    if (asset is null)
                        continue;

                    return asset;
                }
                catch
                {
                    continue;
                }
            }

            throw new UnparsableAssetException(assetText);
        }

        private static readonly IEnumerable<MethodInfo?> _tryParseMethod = Assembly.GetAssembly(typeof(Asset))
                    !.GetTypes()
                    .Where(t => !t.IsAbstract && typeof(Asset).IsAssignableFrom(t))
                    .Select(t => t.GetMethod("TryParse"));
    }
}