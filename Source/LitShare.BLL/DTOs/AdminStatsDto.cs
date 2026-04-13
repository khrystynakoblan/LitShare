namespace LitShare.BLL.DTOs
{
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }

        public int TotalPosts { get; set; }

        public int ActivePosts { get; set; }

        public int TotalComplaints { get; set; }

        public int PendingComplaints { get; set; }

        public List<CityStatDto> TopCities { get; set; } = new List<CityStatDto>();

        public List<GenreStatDto> TopGenres { get; set; } = new List<GenreStatDto>();
    }
}