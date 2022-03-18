using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace pwnctl.ValueObject
{
    public class IpAddress : ValueObject
    {
        private byte[] Bits { get; set; }

        public AddressFamily Version { get; private set; }
        public string Address{ get; private set; }

        private IpAddress(string address)
        {
            // TODO: construct bits array from address text
        }

        public static IpAddress Create(string address)
        {
            return new IpAddress(address);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;

            foreach (var bit in Bits)
            {
                yield return bit;
            }
        }
    }
}
