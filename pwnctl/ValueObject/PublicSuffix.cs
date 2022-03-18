using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.ValueObject
{
    public class PublicSuffix : ValueObject
    {
        public string Suffix { get; private set; }

        private PublicSuffix(string suffix)
        {
            Suffix = suffix;
        }

        public static PublicSuffix Create(string suffix)
        {
            return new PublicSuffix(suffix);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Suffix;
        }
    }
}
