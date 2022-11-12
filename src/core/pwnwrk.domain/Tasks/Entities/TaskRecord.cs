using pwnwrk.domain.Common.BaseClasses;
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

        public string Discriminator { get; private init; }

        private TaskRecord() {}

        public TaskRecord(TaskDefinition definition, Asset asset)
        {
            Discriminator = asset.GetType().Name;

            GetType().GetProperty(Discriminator).SetValue(this, asset);
            asset.Tasks.Add(this);

            QueuedAt = DateTime.UtcNow;
            Definition = definition;
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

        public List<string> Arguments
        {
            get
            {
                List<string> arguments = new();

                var asset = GetType().GetProperty(Discriminator).GetValue(this);
                var assetType = asset.GetType();

                foreach (var param in Definition.Parameters)
                {
                    var prop = assetType.GetProperty(param);
                    if (prop == null)
                        throw new Exception($"Property {param} not found on type {Discriminator}");

                    var arg = prop.GetValue(asset);

                    arguments.Add(arg.ToString());
                }

                return arguments;
            }
        }

        // Interpolate asset arguments into CommandTemplate
        public string Command {
            get
            {
                string command = Definition.CommandTemplate;

                foreach (var arg in Arguments.Distinct())
                {
                    command = command.Replace("{{" + command.Split("{{")[1].Split("}}")[0] + "}}", arg);
                }

                return command;
            }
        }
    }
}