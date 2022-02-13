using pwnctl;
using pwnctl.Persistence;
using pwnctl.Entities;
using pwnctl.Handlers;
using pwnctl.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pwnctl.Services
{
    public class AssetService
    {
        private readonly AssetHandlerMap _assetHandlerMap = new();

        public async Task ProcessAsync(string assetText)
        {
            bool parsed = AssetParser.TryParse(assetText, out Type assetType, out BaseAsset asset);
            if (!parsed)
            {
                return;
            }

            var handler = _assetHandlerMap[assetType];

            await handler.HandleAsync(asset);
        }
    }
}
