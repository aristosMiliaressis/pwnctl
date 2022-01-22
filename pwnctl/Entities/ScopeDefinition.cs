namespace pwnctl.Entities
{
    public class ScopeDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ScopeType Type { get; set; }
        public string Pattern { get; set; }

        public int? ProgramId { get; set; }
        public Program Program { get; set; }

        private ScopeDefinition() {}

        public ScopeDefinition(Program program)
        {
            Program = program;
        }

        public enum ScopeType
        {
            DomainRegex,
            UrlRegex,
            CIDR
        }
    }
}