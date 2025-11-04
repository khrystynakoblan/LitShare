using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitShare.DAL.Repositories
{
    public class UserRepository
    {
        // 1. Поле для збереження контексту
        private readonly LitShareDbContext _context;

        // 2. Конструктор, який "просить" DbContext
        public UserRepository(LitShareDbContext context)
        {
            _context = context;
        }

        // 3. Методи тепер використовують _context, а не створюють новий
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