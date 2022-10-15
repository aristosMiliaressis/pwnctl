using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities.Assets
{
    public class Keyword : BaseAsset
    {
        [UniquenessAttribute]
        public string Word { get; private init; }

        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public Keyword() {}

        public Keyword(Domain domain, string word)
        {
            Domain = domain;
            Word = word;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return Domain.Matches(definition);
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}