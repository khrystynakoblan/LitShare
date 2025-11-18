// -----------------------------------------------------------------------
// <copyright file="Posts.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє модель даних для публікацій (постів) у базі даних.
    /// </summary>
    [Table("posts")]
    public class Posts
    {
        /// <summary>
        /// Gets or sets the unique identifier of the publication.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the post.
        /// </summary>
        [Column("user_id")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the title of the post.
        /// </summary>
        public string Title { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the author of the publication.
        /// </summary>
        public string Author { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the type of deal associated with the post.
        /// </summary>
        [Column("deal_type")]
        public DealType DealType { get; set; }

        /// <summary>
        /// Gets or sets the detailed description of the post or item.
        /// </summary>
        public string Description { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the URL for the post's photo (optional).
        /// </summary>
        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        /// <summary>
        /// Gets or sets the virtual navigation property to the user who created the post.
        /// </summary>
        [ForeignKey("user_id")]
        public virtual Users User { get; set; } = null!; // Виправляє CS8618 для навігаційної властивості

        /// <summary>
        /// Gets or sets the collection of genres associated with the post.
        /// </summary>
        public virtual ICollection<BookGenres> BookGenres { get; set; } = new List<BookGenres>();
    }
}