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
        private readonly LitShareDbContext _context;

        public UserService(LitShareDbContext context)
        {
            _context = context;
        }

        public UserService()
        {
            _context = new LitShareDbContext();
        }

        public List<Users> GetAllUsers() => _context.Users.ToList();

        public Users? GetUserById(int id) => _context.Users.Find(id);

        public void AddUser(string name, string email, string phone, string password, string region, string district, string city)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var newUser = new Users
            {
                Name = name,
                Email = email,
                Phone = phone,
                Password = hashedPassword,
                Region = region,
                District = district,
                City = city
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        public Users? GetUserProfileById(int id)
        {
            return _context.Users
                .Include(u => u.Posts)
                .FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUserPhoto(int userId, string newPhotoUrl)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.PhotoUrl = newPhotoUrl;
                _context.SaveChanges();
            }
        }

        public void UpdateUser(Users updatedUser)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser != null)
            {
                existingUser.Region = updatedUser.Region;
                existingUser.District = updatedUser.District;
                existingUser.City = updatedUser.City;
                existingUser.Phone = updatedUser.Phone;
                existingUser.About = updatedUser.About;
                existingUser.PhotoUrl = updatedUser.PhotoUrl;
                _context.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                throw new Exception("Користувача не знайдено.");

            var posts = _context.posts.Where(p => p.UserId == id).ToList();
            if (posts.Any())
                _context.posts.RemoveRange(posts);

            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}