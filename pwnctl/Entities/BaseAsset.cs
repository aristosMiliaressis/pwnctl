using System;
using System.Collections.Generic;
using System.Text;

namespace pwnctl
{
    public interface IAsset
    {}

    public class BaseAsset : IAsset
    {
        public BaseAsset()
        {
            FoundAt = DateTime.Now;
        }

        public int Id { get; set; }
        public DateTime FoundAt { get; set; }
        public bool InScope { get; set; } = true;
    }
}
