// <copyright file="BookGenres.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents the many-to-many relationship between a Post (book) and a Genre.
    /// This table maps books to their respective genres.
    /// </summary>
    [Table("books_genres")]
    public class BookGenres
    {
        /// <summary>
        /// Gets or sets the foreign key identifier for the post (book).
        /// </summary>
        [Key]
        [Column("post_id")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the genre.
        /// </summary>
        [Key]
        [Column("genre_id")]
        public int GenreId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the related Post entity.
        /// </summary>
        [ForeignKey("post_id")]
        public virtual Posts? Post { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the related Genre entity.
        /// </summary>
        [ForeignKey("genre_id")]
        public virtual Genres? Genre { get; set; }
    }
}