using System.Text.Json;
using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;

namespace pwnwrk.domain.Tasks.Entities
{
    public sealed class TaskRecord : BaseEntity<int>
    {
        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; private init; }

		public int? ReturnCode { get; set; }
		public DateTime QueuedAt { get; private init; }
		public DateTime StartedAt { get; set; }
		public DateTime FinishedAt { get; set; }
		public string Arguments { get; private init; }

        public Host Host { get; private init; }
        public string HostId { get; private init; }

        public Service Service { get; private init; }
        public string ServiceId { get; private init; }

        public Endpoint Endpoint { get; private init; }
        public string EndpointId { get; private init; }

        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public DNSRecord DNSRecord { get; private init; }
        public string DNSRecordId { get; private init; }

        public NetRange NetRange { get; private init; }
        public string NetRangeId { get; private init; }

        public Keyword Keyword { get; private init; }
        public string KeywordId { get; private init; }

        public CloudService CloudService { get; private init; }
        public string CloudServiceId { get; private init; }

        private TaskRecord() {}

        public TaskRecord(TaskDefinition definition, BaseAsset asset)
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
            Arguments = JsonSerializer.Serialize(arguments);
        }

        // Interpolate asset arguments into CommandTemplate
        public string Command {
            get
            {
                string command = Definition.CommandTemplate;

                var args = JsonSerializer.Deserialize<List<string>>(Arguments, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }).Distinct();
                foreach (var arg in args)
                {
                    command = command.Replace("{{" + command.Split("{{")[1].Split("}}")[0] + "}}", arg);
                }

                return command;
            }
        }

        public string WrappedCommand => @$"{Command} | while read assetLine;
do 
    if [[ ${{assetLine::1}} == '{{' ]]; 
    then 
        echo $assetLine | jq -c '.tags += {{""FoundBy"": ""{Definition.ShortName}""}}';
    else 
        echo '{{""asset"":""'$assetLine'"", ""tags"":{{""FoundBy"":""{Definition.ShortName}""}}}}'; 
    fi; 
done | pwnwrk".Replace("\r\n", "").Replace("\n", "");
    }
}