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

        public async Task AddAsync(Complaints complaint)
        {
            await this.context.Complaints.AddAsync(complaint);
        }

        public async Task SaveChangesAsync()
        {
            await this.context.SaveChangesAsync();
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

        public async Task<Complaints?> GetByIdAsync(int id)
        {
            return await this.context.Complaints
                .Include(c => c.Post)
                .Include(c => c.Complainant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task DeleteAsync(Complaints complaint)
        {
            this.context.Complaints.Remove(complaint);
            return Task.CompletedTask;
        }
    }
}