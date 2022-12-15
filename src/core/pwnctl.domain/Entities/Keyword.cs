using pwnctl.kernel.Attributes;
using pwnctl.domain.BaseClasses;

namespace pwnctl.domain.Entities
{
    public sealed class Keyword : Asset
    {
        [EqualityComponent]
        public string Word { get; init; }

        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public Keyword() {}

        public Keyword(Domain domain, string word)
        {
            Domain = domain;
            Word = word;
        }

        public override string ToString()
        {
            return Word;
        }
    }
}