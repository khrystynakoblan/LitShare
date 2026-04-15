namespace LitShare.DAL.Repositories
{
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PostRepository : IPostRepository
    {
        private readonly LitShareDbContext context;

        public PostRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Posts>> GetFilteredAsync(
            string? searchQuery,
            string? city,
            List<int>? genreIds,
            List<string>? dealTypeStrings)
        {
            var query = this.context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lower = searchQuery.ToLower();
                query = query.Where(p =>
                    (p.Title != null && p.Title.ToLower().Contains(lower)) ||
                    (p.Author != null && p.Author.ToLower().Contains(lower)));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var lowerCity = city.ToLower();
                query = query.Where(p =>
                    p.User != null &&
                    p.User.City != null &&
                    p.User.City.ToLower().Contains(lowerCity));
            }

            if (genreIds != null && genreIds.Any())
            {
                query = query.Where(p =>
                    p.BookGenres.Any(bg => genreIds.Contains(bg.GenreId)));
            }

            if (dealTypeStrings != null && dealTypeStrings.Any())
            {
                query = query.Where(p => dealTypeStrings.Contains(p.DealType.ToString().ToLower()));
            }

            return await query.ToListAsync();
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

        public async Task UpdateAsync(Posts post)
        {
            this.context.Posts.Update(post);
            await this.context.SaveChangesAsync();
        }

        public async Task<Posts?> GetByIdWithGenresAsync(int id)
        {
            return await this.context.Posts
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task DeleteAsync(Posts post)
        {
            this.context.Posts.Remove(post);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Posts>> GetAllPostsAsync()
        {
            return await this.context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .ToListAsync();
        }

        public Task DeletePostAsync(Posts post)
        {
            this.context.Posts.Remove(post);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Posts>> GetAllAsync()
        {
            return await this.GetAllPostsAsync();
        }
    }
}