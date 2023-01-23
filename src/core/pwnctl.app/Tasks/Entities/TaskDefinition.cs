using pwnctl.app.Assets.Aggregates;
using pwnctl.domain.ValueObjects;
using pwnctl.kernel.BaseClasses;

namespace pwnctl.app.Tasks.Entities
{
    public sealed class TaskDefinition : Entity<int>
    {
        public string ShortName { get; private init; }
        public string CommandTemplate { get; set; }
        public bool IsActive { get; private init; }
        public int Aggressiveness { get; private init; }
        public AssetClass SubjectClass { get; private set; }
        public string Filter { get; private init; }
        public bool MatchOutOfScope { get; private init; }

        public string Subject { init { SubjectClass = AssetClass.Create(value); } }

        public TaskDefinition() {}

        public bool Matches(AssetRecord record)
        {
            if (SubjectClass.Class != record.Asset.GetType().Name)
                return false;

            if (string.IsNullOrEmpty(Filter))
                return true;

            return PwnInfraContext.FilterEvaluator.Evaluate(Filter, record);
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