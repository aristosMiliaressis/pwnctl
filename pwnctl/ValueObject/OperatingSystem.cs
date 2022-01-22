using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.ValueObject
{
    public class OperatingSystem : ValueObject
    {
        public static OperatingSystem Unknown => new OperatingSystem();

        public OperatingSystem()
            : this("Unknown", "Unknown", "Unknown") 
        {}

        public OperatingSystem(string f, string v, string b)
        {
            Family = f;
            Version = v;
            Build = b;
        }

        public string Family { get; set; }
        public string Version { get; set; }
        public string Build { get; set; }
    }
}
