namespace pwnctl.dto.Mediator;

public abstract class PaginatedViewModel
{
    public List<object> Rows { get; set; } = new();

    public int? Page { get; set; }
    public int? TotalPages { get; set; }
}

public abstract class PaginatedViewModel<TRow> : PaginatedViewModel
{
    public new List<TRow> Rows 
    {
        get
        {
            return base.Rows.Select(r => (TRow)r).ToList();
        }
        set
        {
            base.Rows = value.Select(r => (object)r).ToList();
        }
    }
}