using LitShare.DAL;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;

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
                name = name,
                email = email,
                phone = phone,
                password = hashedPassword,
                region = region,
                district = district,
                city = city
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.password);
        }

        public Users? GetUserProfileById(int id)
        {
            return _context.Users
                .Include(u => u.posts)
                .FirstOrDefault(u => u.id == id);
        }

        public void UpdateUserPhoto(int userId, string newPhotoUrl)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.photo_url = newPhotoUrl;
                _context.SaveChanges();
            }
        }

        public void UpdateUser(Users updatedUser)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.id == updatedUser.id);
            if (existingUser != null)
            {
                existingUser.region = updatedUser.region;
                existingUser.district = updatedUser.district;
                existingUser.city = updatedUser.city;
                existingUser.phone = updatedUser.phone;
                existingUser.about = updatedUser.about;
                existingUser.photo_url = updatedUser.photo_url;
                _context.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.id == id);
            if (user == null)
                throw new Exception("Користувача не знайдено.");

            var posts = _context.posts.Where(p => p.user_id == id).ToList();
            if (posts.Any())
                _context.posts.RemoveRange(posts);

            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}