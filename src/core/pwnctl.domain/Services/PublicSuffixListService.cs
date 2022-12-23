namespace pwnctl.domain.Services;

using pwnctl.domain.ValueObjects;

public interface PublicSuffixListService
{
    static PublicSuffixListService Instance { get; set; }

    PublicSuffix GetPublicSuffix(string name);
}