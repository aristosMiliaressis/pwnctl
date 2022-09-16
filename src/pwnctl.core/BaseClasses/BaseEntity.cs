using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl.core.BaseClasses
{
    public abstract class BaseEntity
    {

    }

    public abstract class BaseEntity<TPKey> : BaseEntity
    {
        protected BaseEntity()
        {
            FoundAt = DateTime.Now;
        }

        public TPKey Id { get; set; }
        public DateTime FoundAt { get; set; }
    }
}
