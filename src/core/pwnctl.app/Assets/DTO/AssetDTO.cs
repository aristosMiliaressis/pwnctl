namespace pwnctl.app.Assets.DTO
{
    /// <summary>
    /// A DTO used to import and export assets along with their associated metadata.
    /// </summary>
    public sealed class AssetDTO
    {
        public string Asset { get; set; }
        public Dictionary<string, object> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}