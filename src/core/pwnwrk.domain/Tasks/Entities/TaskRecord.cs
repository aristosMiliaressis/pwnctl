using pwnwrk.domain.Common.BaseClasses;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Assets.Entities;
using pwnwrk.domain.Tasks.Enums;
using pwnwrk.domain.Tasks.Exceptions;
using System.Text.Json.Serialization;

namespace pwnwrk.domain.Tasks.Entities
{
    public sealed class TaskRecord : Entity<int>
    {
        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; set; }

		public int? ExitCode { get; private set; }
		public DateTime QueuedAt { get; private set; }
		public DateTime StartedAt { get; private set; }
		public DateTime FinishedAt { get; private set; }
        public TaskState State { get; private set; }
        
        public Host Host { get; set; }
        public string HostId { get; private init; }

        public Service Service { get; set; }
        public string ServiceId { get; private init; }

        public Endpoint Endpoint { get; set; }
        public string EndpointId { get; private init; }

        public Domain Domain { get; set; }
        public string DomainId { get; private init; }

        public DNSRecord DNSRecord { get; set; }
        public string DNSRecordId { get; private init; }

        public NetRange NetRange { get; set; }
        public string NetRangeId { get; private init; }

        public Keyword Keyword { get; set; }
        public string KeywordId { get; private init; }

        public CloudService CloudService { get; set; }
        public string CloudServiceId { get; private init; }

        public string Discriminator { get; set; }

        public TaskRecord() {}

        public TaskRecord(TaskDefinition definition, Asset asset)
        {
            State = TaskState.PENDING;
            Definition = definition;

            Discriminator = asset.GetType().Name;
            GetType().GetProperty(Discriminator).SetValue(this, asset);
            asset.Tasks.Add(this);
        }

        public void Queued()
        {
            if (State != TaskState.PENDING)
                throw new TaskStateException("TaskRecord queued when not in PENDING state.");

            State = TaskState.QUEUED;
            QueuedAt = DateTime.UtcNow;
        }

        public void Started()
        {
            if (State != TaskState.QUEUED)
                throw new TaskStateException("TaskRecord started when not in QUEUED state.");

            State = TaskState.RUNNING;
            StartedAt = DateTime.UtcNow;
        }

        public void Finished(int exitCode)
        {
            if (State != TaskState.RUNNING)
                throw new TaskStateException("TaskRecord finished when not in RUNNING state.");

            ExitCode = exitCode;
            State = TaskState.FINISHED;
            FinishedAt = DateTime.UtcNow;
        }

        [JsonIgnore]
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
                        throw new CommandInterpolationException($"Property {param} not found on type {Discriminator}");

                    var arg = prop.GetValue(asset);

                    arguments.Add(arg.ToString());
                }

                return arguments;
            }
        }

        // Interpolate asset arguments into CommandTemplate
        [JsonIgnore]
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