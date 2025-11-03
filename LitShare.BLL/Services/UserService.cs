using LitShare.DAL;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitShare.BLL.Services
{
    public class UserService
    {
        public List<Users> GetAllUsers()
        {
            using (var context = new LitShareDbContext())
            {
                return context.Users.ToList();
            }
        }

        public Users? GetUserById(int id)
        {
            using (var context = new LitShareDbContext())
            {
                return context.Users.Find(id);
            }
        }

        public void AddUser(string name, string email, string password, string region, string district, string city)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new Users
            {
                name = name,
                email = email,
                password = hashedPassword,
                region = region,
                district = district,
                city = city
            };

            using (var context = new LitShareDbContext())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
            }
        }

        // Перевірити користувача (логін)
        public async Task<bool> ValidateUser(string email, string password)
        {
            using (var context = new LitShareDbContext())
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.email == email);

                if (user == null)
                    return false;

                return await Task.Run(() => BCrypt.Net.BCrypt.Verify(password, user.password));
            }
        }
    }
}
