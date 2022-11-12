using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.infra.Persistence.IdGenerators
{
    public sealed class HashIdValueGenerator : StringValueGenerator, IDisposable
    {
        public override string Next(EntityEntry entry) => GenerateHashId(entry.Entity as Asset);
        protected override object NextValue(EntityEntry entry) => GenerateHashId(entry.Entity as Asset);

        private string GenerateHashId(Asset asset)
        {
            var uniqnessValues = asset.GetType()
                        .GetProperties()
                        .Where(p => p.GetCustomAttribute(typeof(UniquenessAttribute)) != null)
                        .OrderBy(p => p.Name)
                        .Select(p => p.GetValue(asset));
            var json = PwnContext.Serializer.Serialize(uniqnessValues);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToHexString(_md5.ComputeHash(bytes));
        }

        public override bool GeneratesTemporaryValues => false;

        public void Dispose()
        {
            _md5.Dispose();
        }

        private MD5 _md5 = MD5.Create();
    }
}