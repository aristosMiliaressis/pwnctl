using pwnwrk.domain.Attributes;
using pwnwrk.domain.BaseClasses;

namespace pwnwrk.domain.Entities.Assets
{
    public class Keyword : BaseAsset
    {
        [UniquenessAttribute]
        public string Word { get; set; }

        public Domain Domain { get; set; }
        public string DomainId { get; set; }

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