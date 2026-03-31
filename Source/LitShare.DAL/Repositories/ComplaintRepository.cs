namespace LitShare.DAL.Repositories
{
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class ComplaintRepository : IComplaintRepository
    {
        private readonly LitShareDbContext context;

        public ComplaintRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Complaints>> GetAllAsync()
        {
            return await this.context.Complaints
                .AsNoTracking()
                .Include(c => c.Post)
                .Include(c => c.Complainant)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }
    }
}