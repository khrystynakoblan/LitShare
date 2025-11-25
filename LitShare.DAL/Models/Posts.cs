// <copyright file="Posts.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a post or listing for a book or item shared by a user.
    /// </summary>
    [Table("posts")]
    public class Posts
    {
        /// <summary>
        /// Gets or sets the unique identifier (primary key) of the post.
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the user who created the post.
        /// </summary>
        [Column("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the title of the post or book.
        /// </summary>
        [Column("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        [Column("author")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the deal type for this listing (e.g., sale, exchange, free).
        /// </summary>
        [Column("deal_type")]
        public DealType DealType { get; set; } // SA1300 Fix

        /// <summary>
        /// Gets or sets the detailed description of the post or item.
        /// </summary>
        [Column("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the cover photo.
        /// </summary>
        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who owns the post.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        /// <summary>
        /// Gets or sets the collection of genre mappings associated with this post.
        /// </summary>
        public virtual ICollection<BookGenres> BookGenres { get; set; } = new List<BookGenres>();
    }
}