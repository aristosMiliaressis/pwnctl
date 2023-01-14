namespace pwnctl.domain.Interfaces;

using pwnctl.domain.ValueObjects;

public interface PublicSuffixListService
{
    static PublicSuffixListService Instance { get; set; }

    PublicSuffix GetSuffix(string name);
}