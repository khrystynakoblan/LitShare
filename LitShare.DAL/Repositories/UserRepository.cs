using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitShare.DAL.Repositories
{
    public class UserRepository
    {
        private readonly LitShareDbContext _context;

        public UserRepository(LitShareDbContext context)
        {
            _context = context;
        }

        public async Task<List<Users>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<Users?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task AddUserAsync(Users user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}