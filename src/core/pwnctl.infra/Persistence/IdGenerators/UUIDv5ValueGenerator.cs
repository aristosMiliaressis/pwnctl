using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using pwnctl.app.Assets.Entities;
using Be.Vlaanderen.Basisregisters.Generators.Guid;

namespace pwnctl.infra.Persistence.IdGenerators
{
    /// <summary>
    /// This class generates UUIDv5 primary keys for assets & AssetRecords by hashing the 
    /// string notation of the asset and effectivly prevents duplicate entries.
    /// </summary>
    public sealed class UUIDv5ValueGenerator : GuidValueGenerator
    {
        private static Guid _assetNamespace = Deterministic.Namespaces.IsoOid;

        public override bool GeneratesTemporaryValues => false;
        public override Guid Next(EntityEntry entry) => Generate(entry.Entity);
        protected override object NextValue(EntityEntry entry) => Generate(entry.Entity);

        private Guid Generate(object asset)
        {
            var textNotation = asset is AssetRecord record
                            ? record.TextNotation
                            : asset.ToString();

            if (string.IsNullOrEmpty(textNotation))
                return default;

            return GenerateByString(textNotation);
        }

        public static Guid GenerateByString(string asset)
        {
            return Deterministic.Create(_assetNamespace, asset, 5);
        }
    }
}