namespace LitShare.BLL.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class GenreService : IGenreService
    {
        private const string GenresCacheKey = "genres_all";

        private readonly IGenreRepository genreRepository;
        private readonly IMemoryCache cache;
        private readonly AppSettings settings;
        private readonly ILogger<GenreService> logger;

        public GenreService(
            IGenreRepository genreRepository,
            IMemoryCache cache,
            IOptions<AppSettings> options,
            ILogger<GenreService> logger)
        {
            this.genreRepository = genreRepository;
            this.cache = cache;
            this.settings = options.Value;
            this.logger = logger;
        }

        public async Task<Result<List<GenreDto>>> GetAllGenresAsync()
        {
            if (this.cache.TryGetValue(GenresCacheKey, out List<GenreDto>? cached) && cached != null)
            {
                this.logger.LogInformation("Genres loaded from cache.");
                return cached;
            }

            this.logger.LogInformation("Genres not in cache. Fetching from database.");

            var genres = await this.genreRepository.GetAllAsync();

            var genreDtos = genres.Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name ?? string.Empty,
            }).ToList();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(this.settings.GenresCacheDurationMinutes),
            };

            this.cache.Set(GenresCacheKey, genreDtos, cacheOptions);

            this.logger.LogInformation(
                "Genres cached for {Minutes} minutes. Count: {Count}.",
                this.settings.GenresCacheDurationMinutes,
                genreDtos.Count);

            return genreDtos;
        }
    }
}