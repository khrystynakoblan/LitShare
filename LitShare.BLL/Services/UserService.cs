// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Services
{
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    public class UserService
    {
        private readonly LitShareDbContext context;

        public UserService(LitShareDbContext context)
        {
            this.context = context;
        }

        public UserService()
        {
            this.context = new LitShareDbContext();
        }

        public List<Users> GetAllUsers() => this.context.Users.ToList();

        public Users? GetUserById(int id) => this.context.Users.Find(id);

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
                city = city,
                role = RoleType.user,
            };

            this.context.Users.Add(newUser);
            this.context.SaveChanges();
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.password);
        }

        public Users? GetUserProfileById(int id)
        {
            return this.context.Users
                .Include(u => u.posts)
                .FirstOrDefault(u => u.id == id);
        }

        public void UpdateUserPhoto(int userId, string newPhotoUrl)
        {
            var user = this.context.Users.Find(userId);
            if (user != null)
            {
                user.photo_url = newPhotoUrl;
                this.context.SaveChanges();
            }
        }

        public void UpdateUser(Users updatedUser)
        {
            var existingUser = this.context.Users.FirstOrDefault(u => u.id == updatedUser.id);
            if (existingUser != null)
            {
                existingUser.region = updatedUser.region;
                existingUser.district = updatedUser.district;
                existingUser.city = updatedUser.city;
                existingUser.phone = updatedUser.phone;
                existingUser.about = updatedUser.about;
                existingUser.photo_url = updatedUser.photo_url;
                this.context.SaveChanges();
            }
        }

        public void DeleteUser(int id)
        {
            var user = this.context.Users.FirstOrDefault(u => u.id == id);
            if (user == null)
            {
                throw new Exception("Користувача не знайдено.");
            }

            var posts = this.context.posts.Where(p => p.user_id == id).ToList();
            if (posts.Any())
            {
                this.context.posts.RemoveRange(posts);
            }

            this.context.Users.Remove(user);
            this.context.SaveChanges();
        }
    }
}