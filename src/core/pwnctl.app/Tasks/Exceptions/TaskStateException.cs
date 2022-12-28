using pwnctl.app.Tasks.Entities;
using pwnctl.app.Tasks.Enums;

namespace pwnctl.app.Tasks.Exceptions;

public sealed class TaskStateException : Exception
{
    public TaskStateException(TaskState original, TaskState attempted)
        : base($"Invalid {nameof(TaskEntry)} state transition from {original} to {attempted}.")
    { }
}