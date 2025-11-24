// <copyright file="BookGenres.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents the join entity for the many-to-many relationship between posts and genres.
    /// This entity maps books to their respective genre classifications.
    /// </summary>
    [Table("books_genres")]
    public class BookGenres
    {
        /// <summary>
        /// Gets or sets the foreign key identifier of the associated post (book).
        /// </summary>
        [Column("post_id")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the associated genre.
        /// </summary>
        [Column("genre_id")]
        public int GenreId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the related Post entity.
        /// </summary>
        [ForeignKey("PostId")]
        public virtual Posts? Post { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the related Genre entity.
        /// </summary>
        [ForeignKey("GenreId")]
        public virtual Genres? Genre { get; set; } 
    }
}