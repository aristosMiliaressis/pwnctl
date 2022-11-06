using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Common.Interfaces;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;

namespace pwnwrk.domain.Tasks.Entities
{
    public sealed class TaskRecord : Entity<int>
    {
        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; private init; }

		public int? ExitCode { get; private set; }
		public DateTime QueuedAt { get; private init; }
		public DateTime StartedAt { get; private set; }
		public DateTime FinishedAt { get; private set; }
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

        public TaskRecord(TaskDefinition definition, Asset asset)
        {
            GetType().GetProperty(asset.GetType().Name + "Id").SetValue(this, asset.Id);

            QueuedAt = DateTime.UtcNow;
            Definition = definition;
            List<object> arguments = new();
            foreach(var param in definition.Parameters)
            {
                if (asset.GetType().GetProperty(param) == null)
                    throw new Exception($"Property {param} not found on type {asset.GetType().Name}");
                var arg = asset.GetType().GetProperty(param).GetValue(asset);
                arguments.Add(arg);
            }
            Arguments = AmbientService<ISerializer>.Instance.Serialize(arguments);
        }

        public void Started()
        {
            StartedAt = DateTime.UtcNow;
        }

        public void Finished(int exitCode)
        {
            ExitCode = exitCode;
            FinishedAt = DateTime.UtcNow;
        }

        // Interpolate asset arguments into CommandTemplate
        public string Command {
            get
            {
                string command = Definition.CommandTemplate;

                var args = AmbientService<ISerializer>.Instance.Deserialize<List<string>>(Arguments).Distinct();
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