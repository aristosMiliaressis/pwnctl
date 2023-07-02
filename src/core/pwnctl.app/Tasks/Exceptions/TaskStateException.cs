using pwnctl.app.Common.Exceptions;
using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Enums;

namespace pwnctl.app.Tasks.Exceptions;

public sealed class TaskStateException : AppException
{
    public TaskStateException(TaskState original, TaskState attempted)
        : base($"Invalid {nameof(TaskRecord)} state transition from {original} to {attempted}.")
    { }
}