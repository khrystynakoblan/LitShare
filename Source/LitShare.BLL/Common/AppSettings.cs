namespace LitShare.BLL.Common
{
    public class AppSettings
    {
        public const string SectionName = "AppSettings";

        public int MinSearchLength { get; set; } = 2;

        public long MaxImageSizeBytes { get; set; } = 5242880;

        public int MaxFavoritesPerUser { get; set; } = 100;

        public int AdminStatsTopCitiesCount { get; set; } = 5;

        public int AdminStatsTopGenresCount { get; set; } = 5;
    }
}