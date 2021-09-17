using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.ValueObject
{
    public class Tag : ValueObject
    {
        public TagSubject Subject { get; set; }
        public string Type { get; set; }
    }

    public enum TagSubject
    {
        Service,
        Endpoint
    }
}
