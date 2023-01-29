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
        private static MD5 _md5 = MD5.Create();
        public override string Next(EntityEntry entry) => GenerateHashId(entry.Entity);
        protected override object NextValue(EntityEntry entry) => GenerateHashId(entry.Entity);

        private string GenerateHashId(object entity)
        {
            return HashIdValueGenerator.Generate(entity);        
        }

        public static string Generate(object entity)
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

            return Convert.ToHexString(_md5.ComputeHash(Encoding.UTF8.GetBytes(asset.ToString())));
        }

        public override bool GeneratesTemporaryValues => false;
    }
}