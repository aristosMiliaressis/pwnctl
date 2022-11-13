using pwnwrk.domain.Assets.Attributes;
using pwnwrk.domain.Assets.DTO;
using pwnwrk.domain.Targets.Entities;
using pwnwrk.domain.Assets.BaseClasses;

namespace pwnwrk.domain.Assets.Entities
{
    public sealed class Keyword : Asset
    {
        [UniquenessAttribute]
        public string Word { get; init; }

        public Domain Domain { get; private init; }
        public string DomainId { get; private init; }

        public Keyword() {}

        public Keyword(Domain domain, string word)
        {
            Domain = domain;
            Word = word;
        }

        internal override bool Matches(ScopeDefinition definition)
        {
            return Domain.Matches(definition);
        }

        public override AssetDTO ToDTO()
        {
            throw new NotImplementedException();
        }
    }
}