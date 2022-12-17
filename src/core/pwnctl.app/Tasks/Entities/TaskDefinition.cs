using pwnctl.app.Common.Interfaces;
using pwnctl.domain.BaseClasses;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskDefinition : Entity<int>
    {
        public string ShortName { get; private init; }
		public string CommandTemplate { get; set; }
		public bool IsActive { get; private init; }
		public int Aggressiveness { get; private init; }
		public string Subject { get; private init; }
        public string Filter { get; private init; }

        public TaskDefinition() {}

        public bool Matches(Asset asset)
        {
            if (Subject != asset.GetType().Name)
                return false;

            if (string.IsNullOrEmpty(Filter))
                return true;

            return FilterEvaluator.Instance.Evaluate(Filter, asset);
        }

        // Extrapolate list of parameter names from CommandTemplate
        public List<string> Parameters
        { 
            get
            {
                List<string> _params = new();

                foreach (var seg in CommandTemplate.Split("}}"))
                {
                    if (!seg.Contains("{{"))
                        continue;

                    _params.Add(seg.Split("{{")[1]);
                }

                return _params;
            }
        }
    }
}