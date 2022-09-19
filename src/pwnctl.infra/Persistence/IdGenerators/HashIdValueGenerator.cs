using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Text;
using System.Reflection;
using Newtonsoft.Json;

namespace pwnctl.infra.Persistence.IdGenerators
{
    public class HashIdValueGenerator : StringValueGenerator, IDisposable
    {
        public override string Next(EntityEntry entry) => GenerateHashId(entry.Entity as BaseAsset);
        protected override object NextValue(EntityEntry entry) => GenerateHashId(entry.Entity as BaseAsset);

        private string GenerateHashId(BaseAsset asset)
        {
            var uniqnessValues = asset.GetType()
                        .GetProperties()
                        .Where(p => p.GetCustomAttribute(typeof(UniquenessAttribute)) != null)
                        .Select(p => p.GetValue(asset));
            var json = JsonConvert.SerializeObject(uniqnessValues);
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToHexString(_md5.ComputeHash(bytes));
        }

        public override bool GeneratesTemporaryValues => false;

        public void Dispose()
        {
            _md5.Dispose();
        }

        private System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();
    }
}