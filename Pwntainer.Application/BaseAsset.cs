using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application
{
    public class BaseAsset
    {
        public BaseAsset()
        {
            FoundAt = DateTime.Now;
        }

        public int Id { get; set; }
        public string TasksRun { get; set; }
        public DateTime FoundAt { get; set; }
    }
}
