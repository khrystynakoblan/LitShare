namespace LitShare.BLL.DTOs
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class VolumeInfo
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("authors")]
        public List<string>? Authors { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}