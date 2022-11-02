namespace pwnctl.dto.Targets.ViewModels;

using pwnwrk.domain.Targets.Entities;

public class TargetListViewModel
{
    public List<Program> Targets { get; init; }

    public TargetListViewModel(List<Program> targets)
    {
        Targets = targets;
    }
}