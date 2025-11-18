// -----------------------------------------------------------------------
// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    // Оскільки використовується BCrypt, припускаємо, що він імпортований.
    // using BCrypt.Net;

    /// <summary>
    /// Надає бізнес-логіку для керування користувачами (CRUD, аутентифікація).
    /// </summary>
    public class UserService
    {
        private readonly LitShareDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with a specified context.
        /// </summary>
        /// <param name="context">The database context instance to use.</param>
        public UserService(LitShareDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class with a new context.
        /// </summary>
        public UserService()
        {
            this.context = new LitShareDbContext();
        }

        /// <summary>
        /// Retrieves a list of all users from the database.
        /// </summary>
        /// <returns>A list of all <see cref="Users"/> entities.</returns>
        public List<Users> GetAllUsers() => this.context.Users.ToList();

        /// <summary>
        /// Retrieves a single user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The <see cref="Users"/> entity, or null if not found.</returns>
        public Users? GetUserById(int id) => this.context.Users.Find(id);

        /// <summary>
        /// Adds a new user to the database after hashing the password.
        /// </summary>
        /// <param name="name">The user's full name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        /// <param name="password">The user's raw password.</param>
        /// <param name="region">The user's region.</param>
        /// <param name="district">The user's district.</param>
        /// <param name="city">The user's city.</param>
        public void AddUser(string name, string email, string phone, string password, string region, string district, string city)
        {
            // BCrypt.Net.BCrypt має бути доступний.
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new Users
            {
                Name = name,
                Email = email,
                Phone = phone,
                Password = hashedPassword,
                Region = region,
                District = district,
                City = city,
            };

            this.context.Users.Add(newUser);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Asynchronously validates user credentials against the stored hashed password.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's raw password.</param>
        /// <returns>A task resulting in true if validation is successful, otherwise false.</returns>
        public async Task<bool> ValidateUser(string email, string password)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == email);

            // Використовуємо BCrypt.Net.BCrypt
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        /// <summary>
        /// Retrieves a user profile by ID, including associated posts.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The <see cref="Users"/> entity with included navigation properties, or null.</returns>
        public Users? GetUserProfileById(int id)
        {
            return this.context.Users
                .Include(u => u.Posts)
                .FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Updates the photo URL for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="newPhotoUrl">The new photo URL to assign.</param>
        public void UpdateUserPhoto(int userId, string newPhotoUrl)
        {
            var user = this.context.Users.Find(userId);

            if (user != null)
            {
                user.PhotoUrl = newPhotoUrl;
                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Updates specific profile details for an existing user.
        /// </summary>
        /// <param name="updatedUser">An entity containing the new values and the ID of the user to update.</param>
        public void UpdateUser(Users updatedUser)
        {
            var existingUser = this.context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);

            if (existingUser != null)
            {
                // Оновлення полів
                existingUser.Region = updatedUser.Region;
                existingUser.District = updatedUser.District;
                existingUser.City = updatedUser.City;
                existingUser.Phone = updatedUser.Phone;
                existingUser.About = updatedUser.About;
                existingUser.PhotoUrl = updatedUser.PhotoUrl;

                this.context.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes a user and all associated posts by ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <exception cref="Exception">Throws an exception if the user is not found.</exception>
        public void DeleteUser(int id)
        {
            var user = this.context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("Користувача не знайдено.");
            }

            var posts = this.context.Posts.Where(p => p.UserId == id).ToList();

            if (posts.Any())
            {
                this.context.Posts.RemoveRange(posts);
            }

            this.context.Users.Remove(user);
            this.context.SaveChanges();
        }
    }
}