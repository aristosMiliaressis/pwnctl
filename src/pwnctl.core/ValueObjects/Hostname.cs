using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.ValueObjects
{
    public class Hostname : ValueObject
    {
        private Hostname(string hostname)
        {
            throw new NotImplementedException();
        }

        public static Hostname Create(string hostname)
        {
            return new Hostname(hostname);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            throw new NotImplementedException();
        }
    }
}
