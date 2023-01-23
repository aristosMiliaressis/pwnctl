using pwnctl.kernel.BaseClasses;
using System.Text;
using System.Security.Cryptography;

namespace pwnctl.domain.BaseClasses
{
    public abstract class Asset : Entity<string>
    {
        private MD5 _md5 = MD5.Create();

        public string UID => Convert.ToHexString(_md5.ComputeHash(Encoding.UTF8.GetBytes(ToString())));

        public abstract override string ToString();
    }
}
