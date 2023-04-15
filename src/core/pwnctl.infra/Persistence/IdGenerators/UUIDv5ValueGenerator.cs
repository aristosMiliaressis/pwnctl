using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Aggregates;
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
        public override Guid Next(EntityEntry entry) => UUIDv5ValueGenerator.Generate(entry.Entity);
        protected override object NextValue(EntityEntry entry) => UUIDv5ValueGenerator.Generate(entry.Entity);

        public static Guid Generate(object entity)
        {
            Asset asset = null;

            if (entity is AssetRecord)
            {
                var record = entity as AssetRecord;
                asset = record.Asset;
            }
            else
            {
                asset = entity as Asset;
            }

            return Deterministic.Create(_assetNamespace, asset.ToString(), 5);
        }
    }
}