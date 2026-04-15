using LitShare.DAL.Context;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore; // ЦЕЙ РЯДОК ОБОВ'ЯЗКОВИЙ ДЛЯ AnyAsync

namespace LitShare.DAL.Repositories
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly LitShareDbContext context;

        public ExchangeRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(ExchangeRequest request)
        {
            await this.context.ExchangeRequests.AddAsync(request);
        }

        public async Task<bool> ExistsAsync(int senderId, int postId)
        {
            return await this.context.ExchangeRequests
                .AnyAsync(r => r.SenderId == senderId && r.PostId == postId);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ExchangeRequest>> GetBySenderIdAsync(int senderId)
        {
            return await this.context.ExchangeRequests
                .Include(r => r.Post)
                .Where(r => r.SenderId == senderId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}