using System;
using System.Collections.Generic;
using System.Text;
using pwnctl;
using pwnctl.Services;

namespace pwnctl
{
    public interface IAsset
    {}

    public abstract class BaseAsset : IAsset
    {
        protected BaseAsset()
        {
            FoundAt = DateTime.Now;
            InScope = ScopeService.Singleton.IsInScope(this);
        }

        public int Id { get; set; }
        public DateTime FoundAt { get; set; }
        public bool InScope { get; set; }
    }
}
