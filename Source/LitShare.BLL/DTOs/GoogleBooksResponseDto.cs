namespace LitShare.BLL.DTOs
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class GoogleBooksResponseDto
    {
        [JsonPropertyName("items")]
        public List<GoogleBookItem>? Items { get; set; }
    }
}