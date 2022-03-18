using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.ValueObject
{
    public class Tag : ValueObject
    {
        public TagSubject Subject { get; private set; }
        public string Type { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Subject;
            yield return Type;
        }
    }

    public enum TagSubject
    {
        Service,
        Endpoint
    }
}
