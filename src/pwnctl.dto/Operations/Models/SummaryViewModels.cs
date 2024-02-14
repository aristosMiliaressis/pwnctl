using pwnctl.app.Operations.Enums;

namespace pwnctl.dto.Operations.Models;

public sealed class SummaryViewModel
{
    public string Name { get; set; }
    public string ScopeName { get; set; }
    public OperationType Type { get; set; }
    public OperationState State { get; set; }
    public int CurrentPhase { get; set; }
    public DateTime? InitializedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    //schedule

    public int TagCount { get; set; }
    public int InScopeRangesCount { get; set; }
    public int InScopeHostCount { get; set; }
    public int InScopeDomainCount { get; set; }
    public int InScopeRecordCount { get; set; }
    public int InScopeServiceCount { get; set; }
    public int InScopeEndpointCount { get; set; }
    public int InScopeVirtualHostCount { get; set; }
    public int InScopeParamCount { get; set; }
    public int InScopeEmailCount { get; set; }
    public int QueuedTaskCount { get; set; }
    public int RunningTaskCount { get; set; }
    public int FinishedTaskCount { get; set; }
    public int FailedTaskCount { get; set; }
    public int CanceledTaskCount { get; set; }
    public int TimedOutTaskCount { get; set; }
    public DateTime? FirstTask { get; set; }
    public DateTime? LastTask { get; set; }
    public DateTime? LastFinishedTask { get; set; }

    public List<TaskDefinitionDetails> TaskDetails { get; set; }

    public class TaskDefinitionDetails 
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int RunCount { get; set; }
        public TimeSpan Duration { get; set; }
        public int Findings { get; set; }
        public bool ShortLived { get; set; }
    }
}