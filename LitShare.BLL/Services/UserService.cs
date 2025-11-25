// <copyright file="UserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides services for user management, authentication, and profile interaction.
    /// </summary>
    public class UserService
    {
        private readonly LitShareDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="context">The database context (optional, defaults to a new instance if null).</param>
        public UserService(LitShareDbContext? context = null)
        {
            this.context = context ?? new LitShareDbContext();
        }

        /// <summary>
        /// Retrieves a list of all users from the database.
        /// </summary>
        /// <returns>A list of all <see cref="Users"/> entities.</returns>
        public List<Users> GetAllUsers() => this.context.Users.ToList();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The <see cref="Users"/> entity, or null if not found.</returns>
        public Users? GetUserById(int id) => this.context.Users.Find(id);

        /// <summary>
        /// Adds a new user to the database with a hashed password.
        /// </summary>
        /// <param name="name">The user's full name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phone">The user's phone number.</param>
        /// <param name="password">The plain-text password to be hashed.</param>
        /// <param name="region">The user's region/oblast.</param>
        /// <param name="district">The user's district/raion.</param>
        /// <param name="city">The user's city/town.</param>
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
                City = city,
                Role = RoleType.User,
            };

            this.context.Users.Add(newUser);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Validates user credentials by checking the email and verifying the password hash.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The plain-text password provided by the user.</param>
        /// <returns>A Task resolving to true if validation is successful, otherwise false.</returns>
        public async Task<bool> ValidateUser(string email, string password)
        {
            var user = await this.context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }

        /// <summary>
        /// Retrieves the user profile including related posts.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>The user entity including posts, or null.</returns>
        public Users? GetUserProfileById(int id)
        {
            return this.context.Users
                .Include(u => u.Posts)
                .FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Updates the user's profile photo URL.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="newPhotoUrl">The new photo URL.</param>
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
        /// Updates specific profile fields (location, phone, about) of an existing user.
        /// </summary>
        /// <param name="updatedUser">The entity containing the new data.</param>
        public void UpdateUser(Users updatedUser)
        {
            var existingUser = this.context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
            if (existingUser != null)
            {
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
        /// Deletes a user and all associated posts from the database.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <exception cref="Exception">Thrown if the user is not found.</exception>
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