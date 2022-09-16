using pwnctl.core.BaseClasses;

namespace pwnctl.core.Entities
{
    public class TaskDefinition : BaseEntity<int>
    {
        public string ShortName { get; set; }
		public string CommandTemplate { get; set; }
		public bool IsActive { get; set; }
		public int Aggressiveness { get; set; }
		public string Subject { get; set; }
        public string Filter { get; set; }

        public TaskDefinition() {}

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