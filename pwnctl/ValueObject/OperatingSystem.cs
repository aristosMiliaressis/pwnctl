using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.ValueObject
{
    public class OperatingSystem : ValueObject
    {
        public static OperatingSystem Unknown => Create("Unknown", "Unknown", "Unknown");

        public string Family { get; private set; }
        public string Version { get; private set; }
        public string Build { get; private set; }

        private OperatingSystem(string f, string v, string b)
        {
            Family = f;
            Version = v;
            Build = b;
        }

        public static OperatingSystem Create(string f, string v, string b)
        {
            return new OperatingSystem(f, v, b);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Family;
            yield return Version;
            yield return Build;
        }
    }
}
