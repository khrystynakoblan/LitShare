// <copyright file="Genres.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a distinct genre classification for books/posts.
    /// </summary>
    [Table("genres")]
    public class Genres
    {
        /// <summary>
        /// Gets or sets the unique identifier (primary key) of the genre.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        // Змінено на string? для уникнення попередження CS8618, оскільки властивість Name не ініціалізована.
        public string? Name { get; set; }
    }
}