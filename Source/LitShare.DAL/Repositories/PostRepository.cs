using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LitShare.DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly LitShareDbContext context;

        public PostRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Posts>> GetAllAsync()
        {
            return await this.context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task AddAsync(Posts post)
        {
            await this.context.Posts.AddAsync(post);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task<Posts?> GetByIdAsync(int id)
        {
            return await this.context.Posts
                .Include(p => p.BookGenres)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Posts>> GetByUserIdAsync(int userId)
        {
            return await this.context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public Task UpdateAsync(Posts post)
        {
            this.context.Posts.Update(post);
            return Task.CompletedTask;
        }

        public async Task<Posts?> GetByIdWithGenresAsync(int id)
        {
            return await this.context.Posts
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}