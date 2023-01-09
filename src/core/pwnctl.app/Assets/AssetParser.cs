using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Exceptions;

using System.Reflection;

namespace pwnctl.app.Assets
{
    public static class AssetParser
    {
        public static Asset Parse(string assetText)
        {
            ArgumentNullException.ThrowIfNull(assetText, nameof(assetText));

            assetText = assetText.Trim();

            object[] parameters = new object[] { assetText, null };
            foreach (var tryParseMethod in _tryParseMethod)
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

                return (Asset)parameters[1];
            }

            throw new UnparsableAssetException(assetText);
        }

        private static readonly IEnumerable<MethodInfo> _tryParseMethod = Assembly.GetAssembly(typeof(Asset))
                                                .GetTypes()
                                                .Where(t => !t.IsAbstract && typeof(Asset).IsAssignableFrom(t))
                                                .Select(t => t.GetMethod("TryParse"));
    }
}