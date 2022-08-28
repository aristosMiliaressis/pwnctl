using Newtonsoft.Json;
using pwnctl.core.BaseClasses;
using pwnctl.core.Entities.Assets;

namespace pwnctl.core.Entities
{
    public class Task : BaseEntity
    {
        public int DefinitionId { get; set; }
        public TaskDefinition Definition { get; set; }

		public int? ReturnCode { get; set; }
		public DateTime QueuedAt { get; set; }
		public DateTime StartedAt { get; set; }
		public DateTime FinishedAt { get; set; }
		public string Arguments { get; set; }

        public Host Host { get; set; }
        public int? HostId { get; set; }

        public Service Service { get; set; }
        public int? ServiceId { get; set; }

        public Endpoint Endpoint { get; set; }
        public int? EndpointId { get; set; }

        public Domain Domain { get; set; }
        public int? DomainId { get; set; }

        public DNSRecord DNSRecord { get; set; }
        public int? DNSRecordId { get; set; }

        public NetRange NetRange { get; set; }
        public int? NetRangeId { get; set; }

        private Task() {}

        public Task(TaskDefinition definition, BaseAsset asset)
        {
            GetType().GetProperty(asset.GetType().Name + "Id").SetValue(this, asset.Id);

            QueuedAt = DateTime.Now;
            Definition = definition;
            List<object> arguments = new();
            foreach(var param in definition.Parameters)
            {
                if (asset.GetType().GetProperty(param) == null)
                    throw new Exception($"Property {param} not found on type {asset.GetType().Name}");
                var arg = asset.GetType().GetProperty(param).GetValue(asset);
                arguments.Add(arg);
            }
            Arguments = JsonConvert.SerializeObject(arguments);
        }

        public string Command {
            get
            {
                string command = Definition.CommandTemplate;

                var args = JsonConvert.DeserializeObject<List<string>>(Arguments).Distinct();
                foreach (var arg in args)
                {
                    command = command.Replace("{{" + command.Split("{{")[1].Split("}}")[0] + "}}", arg);
                }

                return command;
            }
        }
    }
}