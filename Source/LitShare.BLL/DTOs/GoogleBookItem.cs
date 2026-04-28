namespace LitShare.BLL.DTOs
{
    using System.Text.Json.Serialization;
    using static Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal.PgTableValuedFunctionExpression;

    public class GoogleBookItem
    {
        [JsonPropertyName("volumeInfo")]
        public VolumeInfo? VolumeInfo { get; set; }
    }
}