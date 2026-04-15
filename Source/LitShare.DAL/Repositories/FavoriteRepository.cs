namespace LitShare.DAL.Repositories
{
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly LitShareDbContext context;

        public FavoriteRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Favorites>> GetByUserIdAsync(int userId)
        {
            return await this.context.Favorites
                .AsNoTracking()
                .Include(f => f.Post)
                    .ThenInclude(p => p!.User)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, int postId)
        {
            return await this.context.Favorites
                .AsNoTracking()
                .AnyAsync(f => f.UserId == userId && f.PostId == postId);
        }

        public async Task AddAsync(Favorites favorite)
        {
            await this.context.Favorites.AddAsync(favorite);
        }

        public Task RemoveAsync(Favorites favorite)
        {
            this.context.Favorites.Remove(favorite);
            return Task.CompletedTask;
        }

        public async Task<Favorites?> GetAsync(int userId, int postId)
        {
            return await this.context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PostId == postId);
        }

        public async Task<HashSet<int>> GetFavoritePostIdsAsync(int userId)
        {
            var ids = await this.context.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == userId)
                .Select(f => f.PostId)
                .ToListAsync();

            return new HashSet<int>(ids);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }
    }
}