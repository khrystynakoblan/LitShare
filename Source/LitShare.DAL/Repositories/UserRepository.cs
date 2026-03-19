namespace LitShare.DAL.Repositories
{
    using LitShare.DAL.Context;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class UserRepository : IUserRepository
    {
        private readonly LitShareDbContext context;

        public UserRepository(LitShareDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await this.context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email);
        }

        public async Task AddAsync(Users user)
        {
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
        }

        public async Task<Users?> GetByEmailAsync(string email)
        {
            return await this.context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}