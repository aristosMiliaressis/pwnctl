using Pwntainer.Application.ValueObject;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class Host : BaseAsset
    {
        public string IP { get; set; }

        public OperatingSystem OperatingSystem { get; set; }
    }
}
