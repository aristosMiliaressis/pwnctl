using pwnctl.kernel.BaseClasses;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Tasks.Exceptions;
using System.Text.Json.Serialization;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskEntry : Entity<int>
    {
        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; set; }

        public int? ExitCode { get; private set; }
        public DateTime QueuedAt { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime FinishedAt { get; private set; }
        public TaskState State { get; private set; }

        public AssetRecord Record { get; set; }
        public string RecordId { get; private init; }

        private TaskEntry() {}

        public TaskEntry(TaskDefinition definition, AssetRecord record)
        {
            State = TaskState.PENDING;
            Definition = definition;
            Record = record;
        }

        public void Queued()
        {
            if (State != TaskState.PENDING)
                throw new TaskStateException(State, TaskState.QUEUED);

            State = TaskState.QUEUED;
            QueuedAt = DateTime.UtcNow;
        }

        public void Started()
        {
            if (State != TaskState.QUEUED && State != TaskState.RUNNING)
                throw new TaskStateException(State, TaskState.RUNNING);

            State = TaskState.RUNNING;
            StartedAt = DateTime.UtcNow;
        }

        public void Finished(int exitCode)
        {
            if (State != TaskState.RUNNING)
                throw new TaskStateException(State, TaskState.FINISHED);

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

                var assetType = Record.Asset.GetType();

                foreach (var param in Definition.Parameters)
                {
                    var prop = assetType.GetProperty(param);
                    if (prop == null)
                        throw new CommandInterpolationException($"Property {param} not found on type {assetType.Name}");

                    var arg = prop.GetValue(Record.Asset);

                    arguments.Add((string)arg);
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

        [JsonIgnore]
        public string WrappedCommand => @$"{Command} | while read assetLine;
do
  if [[ ${{assetLine::1}} == '{{' ]];
  then
    echo $assetLine | jq -c '.FoundBy = ""{Definition.ShortName}""';
  else
    echo '{{""asset"":""'$assetLine'"", ""FoundBy"":""{Definition.ShortName}""}}';
  fi; 
done".Replace("\r\n", "").Replace("\n", "");
    }
}