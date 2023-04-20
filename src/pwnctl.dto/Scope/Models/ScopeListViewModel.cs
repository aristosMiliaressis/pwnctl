namespace pwnctl.dto.Scope.Models;

using pwnctl.app.Scope.Entities;

public sealed class ScopeListViewModel
{
    public IEnumerable<ScopeRequestModel> Scope { get; init; }

    public ScopeListViewModel() { }

    public ScopeListViewModel(List<ScopeAggregate> scope)
    {
        Scope = scope.Select(s => new ScopeRequestModel(s));
    }
}