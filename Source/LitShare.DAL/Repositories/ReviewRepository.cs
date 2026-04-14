namespace LitShare.DAL.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class ReviewRepository : IReviewRepository
    {
        private readonly LitShareDbContext context;

        public ReviewRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(Reviews review)
        {
            await this.context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Reviews>> GetByReviewedUserIdAsync(int reviewedUserId)
        {
            return await this.context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.ReviewedUserId == reviewedUserId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int reviewerId, int reviewedUserId)
        {
            return await this.context.Reviews
                .AnyAsync(r => r.ReviewerId == reviewerId && r.ReviewedUserId == reviewedUserId);
        }
    }
}