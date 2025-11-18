// -----------------------------------------------------------------------
// <copyright file="BookGenre.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore; // Потрібно для [Keyless]

    /// <summary>
    /// Represents a link table between Books (Posts) and Genres.
    /// Used for many-to-many relationship mapping in the database.
    /// </summary>
    [Table("books_genres")]
    [Keyless] // Додано для коректної роботи з EF Core, якщо це join table без Primary Key
    public class BookGenre
    {
        /// <summary>
        /// Gets or sets the foreign key of the linked Post (Book).
        /// </summary>
        [Column("post_id")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key of the linked Genre.
        /// </summary>
        [Column("genre_id")]
        public int GenreId { get; set; }

        // Додаткові порожні рядки між групами властивостей можуть бути додані тут,
        // якщо це вимагається SA1516.

        /// <summary>
        /// Gets or sets the navigation property for the Post (Book).
        /// </summary>
        [ForeignKey("post_id")]
        public virtual Posts? Post { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the Genre.
        /// </summary>
        [ForeignKey("genre_id")]
        public virtual Genres? Genre { get; set; }
    }
}