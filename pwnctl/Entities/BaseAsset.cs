using System;
using System.Collections.Generic;
using System.Text;
using pwnctl;
using pwnctl.Services;

namespace pwnctl
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

    public interface IAsset
    {}

    public abstract class BaseAsset : BaseEntity, IAsset
    {
        protected BaseAsset()
        {
            InScope = ScopeService.Singleton.IsInScope(this);
        }

        public new int Id { get; set; }
        public new DateTime FoundAt { get; set; }
        public bool InScope { get; set; }
        public bool IsRoutable { get; set; }
    }
}
