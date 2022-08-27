using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.core.BaseClasses
{
    public abstract class BaseEntity
    {
        protected BaseEntity()
        {
            FoundAt = DateTime.Now;
        }

        public int Id { get; set; }
        public DateTime FoundAt { get; set; }
    }
}
