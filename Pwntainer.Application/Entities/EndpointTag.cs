using Pwntainer.Application.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pwntainer.Application.Entities
{
    public class EndpointTag : BaseAsset
    {
        public int Id { get; set; }

        public int EndpointId { get; set; }
        public Endpoint Endpoint { get; set; }

        public Tag Tag { get; set; }
    }
}
