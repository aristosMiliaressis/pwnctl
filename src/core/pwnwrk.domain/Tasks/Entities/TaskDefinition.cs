using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Tasks.Entities
{
    public sealed class TaskDefinition : BaseEntity<int>
    {
        public string ShortName { get; private init; }
		public string CommandTemplate { get; private init; }
		public bool IsActive { get; private init; }
		public int Aggressiveness { get; private init; }
		public string Subject { get; private init; }
        public string Filter { get; private init; }

        public TaskDefinition() {}

        // Extrapolate list of parameter names from CommandTemplate
        public List<string> Parameters
        { 
            get
            {
                List<string> _params = new();

                var segments = CommandTemplate.Split("}}");
                foreach (var seg in segments)
                {
                    if (seg.Split("{{").Count() ==1)
                        continue;

                    _params.Add(seg.Split("{{")[1]);
                }

                return _params;
            }
        }
    }
}