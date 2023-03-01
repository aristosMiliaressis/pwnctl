namespace pwnctl.dto.Db.ViewModels;

public sealed class SummaryViewModel
{
    public int NetworkRangeCount { get; set; }
    public int HostCount { get; set; }
    public int DomainCount { get; set; }
    public int RecordCount { get; set; }
    public int SocketCount { get; set; }
    public int HttpEndpointCount { get; set; }
    public int HttpParamCount { get; set; }
    public int EmailCount { get; set; }
    public int TagCount { get; set; }
    public int InScopeRangesCount { get; set; }
    public int InsCopeHostCount { get; set; }
    public int InScopeDomainCount { get; set; }
    public int InScopeRecordCount { get; set; }
    public int InScopeServiceCount { get; set; }
    public int InScopeEndpointCount { get; set; }
    public int InScopeParamCount { get; set; }
    public int InScopeEmailCount { get; set; }
    public int PendingTaskCount { get; set; }
    public int QueuedTaskCount { get; set; }
    public int RunningTaskCount { get; set; }
    public int FinishedTaskCount { get; set; }
    public DateTime? FirstTask { get; set; }
    public DateTime? LastTask { get; set; }
    public DateTime? LastFinishedTask { get; set; }

    public List<TaskDefinitionDetails> TaskDetails { get; set; }

    public class TaskDefinitionDetails 
    {
        public string ShortName { get; set; }
        public int Count { get; set; }
        public TimeSpan Duration { get; set; }
        public int Findings { get; set; }
    }
}