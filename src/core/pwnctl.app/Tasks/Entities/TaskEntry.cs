using pwnctl.kernel.BaseClasses;
using pwnctl.app.Assets.Aggregates;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Tasks.Exceptions;
using pwnctl.app.Operations.Entities;
using System.Text.Json.Serialization;
using pwnctl.app.Common.Extensions;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskEntry : Entity<int>
    {
        public int? ExitCode { get; private set; }
        public DateTime QueuedAt { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime FinishedAt { get; private set; }
        public TaskState State { get; private set; }

        public int OperationId { get; private init; }
        public Operation Operation { get; set; }

        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; set; }

        public AssetRecord Record { get; private init; }
        public Guid RecordId { get; private init; }

        private TaskEntry() {}

        public TaskEntry(Operation operation, TaskDefinition definition, AssetRecord record)
        {
            State = TaskState.QUEUED;
            QueuedAt = DateTime.UtcNow;
            Operation = operation;
            OperationId = operation.Id;
            Definition = definition;
            DefinitionId = definition.Id;
            Record = record;
        }

        public void Started()
        {
            if (State == TaskState.FINISHED)
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

        // Interpolate asset arguments into CommandTemplate
        [JsonIgnore]
        public string Command => Definition.CommandTemplate.Interpolate(Record.Asset);
    }
}