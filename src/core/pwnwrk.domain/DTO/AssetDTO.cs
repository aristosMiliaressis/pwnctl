namespace pwnwrk.domain.DTO
{
    public class AssetDTO
    {
        public string Asset { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}