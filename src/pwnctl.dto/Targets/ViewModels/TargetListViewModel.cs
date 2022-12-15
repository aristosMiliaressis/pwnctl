namespace pwnctl.dto.Targets.ViewModels;

using pwnctl.app.Scope.Entities;

public sealed class TargetListViewModel
{
    public List<Program> Targets { get; init; }

    public TargetListViewModel(List<Program> targets)
    {
        Targets = targets;
    }
}