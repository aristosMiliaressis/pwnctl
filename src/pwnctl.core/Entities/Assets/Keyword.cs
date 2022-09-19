using pwnctl.core.Attributes;
using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities.Assets
{
    public class Keyword : BaseAsset
    {
        [UniquenessAttribute]
        public string Word { get; set; }

        public Keyword() {}

        public Keyword(string word)
        {
            Word = word;
        }

        public override bool Matches(ScopeDefinition definition)
        {
            return true;
        }

        public override string ToJson()
        {
            throw new NotImplementedException();
        }
    }
}