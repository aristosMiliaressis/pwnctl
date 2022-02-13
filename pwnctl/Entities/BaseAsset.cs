using System;
using System.Collections.Generic;
using System.Text;
using pwnctl.Services;

namespace pwnctl
{
    public interface IAsset
    {}

    public class BaseAsset : IAsset
    {
        public BaseAsset()
        {
            FoundAt = DateTime.Now;
            InScope = ScopeService.Instance.IsInScope(this);
        }

        public int Id { get; set; }
        public DateTime FoundAt { get; set; }
        public bool InScope { get; set; }
    }
}
