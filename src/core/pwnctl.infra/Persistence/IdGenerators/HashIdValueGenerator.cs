using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using pwnctl.domain.BaseClasses;
using pwnctl.app.Assets.Aggregates;

namespace pwnctl.infra.Persistence.IdGenerators
{
    public sealed class HashIdValueGenerator : StringValueGenerator
    {
        public override string Next(EntityEntry entry) => GenerateHashId(entry.Entity);
        protected override object NextValue(EntityEntry entry) => GenerateHashId(entry.Entity);

        private string GenerateHashId(object entity)
        {
            Asset asset  = null;

            if (entity is AssetRecord)
            {
                var record = entity as AssetRecord;
                asset = record.Asset;
            }
            else
            {
                asset = entity as Asset;
            }

            return asset.UID;         
        }

        public override bool GeneratesTemporaryValues => false;
    }
}