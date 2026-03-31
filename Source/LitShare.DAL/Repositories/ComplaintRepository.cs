namespace LitShare.DAL.Repositories
{
    using System.Threading.Tasks;
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;

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
    }
}