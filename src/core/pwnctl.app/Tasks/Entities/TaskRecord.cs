using pwnctl.kernel.BaseClasses;
using pwnctl.app.Assets.Entities;
using pwnctl.app.Tasks.Enums;
using pwnctl.app.Tasks.Exceptions;
using pwnctl.app.Operations.Entities;
using System.Text.Json.Serialization;
using pwnctl.app.Common.Extensions;
using pwnctl.kernel;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskRecord : Entity<int>
    {
        public DateTime QueuedAt { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime FinishedAt { get; private set; }
        public TaskState State { get; private set; }
        public int RunCount { get; private set; } = 0;
        public int? ExitCode { get; private set; }
        public string? Stderr { get; set; }

        public int OperationId { get; private init; }
        public Operation Operation { get; set; }

        public int DefinitionId { get; private init; }
        public TaskDefinition Definition { get; set; }

        public AssetRecord Record { get; set; }
        public Guid RecordId { get; set; }

        private TaskRecord() {}

        public TaskRecord(Operation operation, TaskDefinition definition, AssetRecord record)
        {
            State = TaskState.QUEUED;
            QueuedAt = SystemTime.UtcNow();
            OperationId = operation.Id;
            Definition = definition;
            DefinitionId = definition.Id;
            Record = record;
            RecordId = record.Id;
        }

        public void Started()
        {
            if (State == TaskState.FINISHED)
                throw new TaskStateException(State, TaskState.RUNNING);

            State = TaskState.RUNNING;
            StartedAt = SystemTime.UtcNow();
            RunCount++;
        }

        public void Finished(int exitCode, string stderr)
        {
            if (State != TaskState.RUNNING)
                throw new TaskStateException(State, TaskState.FINISHED);

            ExitCode = exitCode;
            State = TaskState.FINISHED;
            FinishedAt = SystemTime.UtcNow();
            Stderr = stderr;
        }

        public void Failed()
        {
            if (State != TaskState.RUNNING)
                throw new TaskStateException(State, TaskState.FAILED);

            State = TaskState.FAILED;
            FinishedAt = SystemTime.UtcNow();
        }

        // Interpolate asset arguments into CommandTemplate
        [JsonIgnore]
        public string Command => Definition.CommandTemplate.Interpolate(Record.Asset);
    }
}
