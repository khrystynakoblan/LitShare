namespace LitShare.BLL.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;

    public class GenreService : IGenreService
    {
        private readonly IGenreRepository genreRepository;
        private readonly ILogger<GenreService> logger;

        public GenreService(IGenreRepository genreRepository, ILogger<GenreService> logger)
        {
            this.genreRepository = genreRepository;
            this.logger = logger;
        }

        public async Task<Result<List<GenreDto>>> GetAllGenresAsync()
        {
            this.logger.LogInformation("Fetching all genres from repository.");
            var genres = await this.genreRepository.GetAllAsync();

            if (genres == null)
            {
                this.logger.LogWarning("Repository returned null for genres.");
                return Result<List<GenreDto>>.Failure("Не вдалося завантажити список жанрів.");
            }

            var genreDtos = genres.Select(g => new GenreDto
            {
                Id = g.Id,
                Name = g.Name ?? string.Empty
            }).ToList();

            return genreDtos;
        }
    }
}