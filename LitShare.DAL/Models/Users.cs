// <copyright file="Users.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using NpgsqlTypes;

    /// <summary>
    /// Defines the possible roles a user can have within the application.
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Standard application user role. Maps to 'user' in the database.
        /// </summary>
        [PgName("user")]
        User,

        /// <summary>
        /// Administrative user role with elevated privileges. Maps to 'admin' in the database.
        /// </summary>
        [PgName("admin")]
        Admin,
    }

    /// <summary>
    /// Represents a registered user account in the application.
    /// </summary>
    [Table("users")]
    public class Users
    {
        /// <summary>
        /// Gets or sets the unique identifier (primary key) of the user.
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        [Column("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user (used for login/contact).
        /// </summary>
        [Column("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number for user contact.
        /// </summary>
        [Column("phone")]
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// </summary>
        [Column("password")]
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets optional biographical information about the user.
        /// </summary>
        [Column("about")]
        public string? About { get; set; }

        /// <summary>
        /// Gets or sets the user's administrative region or oblast.
        /// </summary>
        [Column("region")]
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the user's district (raion).
        /// </summary>
        [Column("district")]
        public string? District { get; set; }

        /// <summary>
        /// Gets or sets the user's current city or town.
        /// </summary>
        [Column("city")]
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the administrative role or user group.
        /// </summary>
        [Column("role")]
        public RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile photo.
        /// </summary>
        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the collection of posts created by this user.
        /// </summary>
        public ICollection<Posts>? Posts { get; set; }
    }
}