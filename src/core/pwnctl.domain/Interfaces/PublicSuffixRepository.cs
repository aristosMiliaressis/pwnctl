namespace pwnctl.domain.Interfaces;

using pwnctl.domain.ValueObjects;

public interface PublicSuffixRepository
{
    static PublicSuffixRepository Instance { get; set; }

    public List<PublicSuffix> List();
}