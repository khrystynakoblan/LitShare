using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LitShare.DAL.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly LitShareDbContext context;

        public GenreRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Genres>> GetAllAsync()
        {
            return await this.context.Genres.ToListAsync();
        }
    }
}