using pwnwrk.domain.Assets.Interfaces;
using pwnwrk.domain.Assets.BaseClasses;
using pwnwrk.domain.Common.BaseClasses;

namespace pwnwrk.domain.Tasks.Entities
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

            return IFilterEvaluator.Instance.Evaluate(Filter, asset);
        }

        // Extrapolate list of parameter names from CommandTemplate
        public List<string> Parameters
        { 
            get
            {
                List<string> _params = new();

                var segments = CommandTemplate.Split("}}");
                foreach (var seg in segments)
                {
                    if (seg.Split("{{").Count() == 1)
                        continue;

                    _params.Add(seg.Split("{{")[1]);
                }

                return _params;
            }
        }
    }
}