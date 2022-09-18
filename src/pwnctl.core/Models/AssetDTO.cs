namespace pwnctl.core.Models
{
    public class AssetDTO
    {
        public string Asset { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}