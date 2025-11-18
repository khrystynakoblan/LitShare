// -----------------------------------------------------------------------
// <copyright file="Users.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє модель даних для користувача.
    /// </summary>
    [Table("users")]
    public class Users
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [Key]
        [Column("id")] // ВАЖЛИВО: мапінг на маленьку літеру 'id'
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [Column("phone")]
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password hash of the user.
        /// </summary>
        [Column("password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's information about themselves (optional).
        /// </summary>
        [Column("about")]
        public string? About { get; set; }

        /// <summary>
        /// Gets or sets the region of residence.
        /// </summary>
        [Column("region")]
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the district of residence.
        /// </summary>
        [Column("district")]
        public string District { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of residence.
        /// </summary>
        [Column("city")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL for the user's profile photo (optional).
        /// </summary>
        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the collection of posts created by the user.
        /// </summary>
        public virtual ICollection<Posts>? Posts { get; set; }
    }
}