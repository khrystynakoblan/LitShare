// <copyright file="BookDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.DTOs
{
    /// <summary>
    /// Represents a simplified view of a post or book for display in the application layer.
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the book/post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book/post.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public string? Author { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the book/post.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets a string listing all genres associated with the book.
        /// </summary>
        public string? Genre { get; set; }

        /// <summary>
        /// Gets or sets the type of transaction (e.g., "Exchange" or "Donation").
        /// </summary>
        public string? DealType { get; set; }

        /// <summary>
        /// Gets or sets the geographical location of the user/book.
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the book's cover image.
        /// </summary>
        public string? ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who owns the book/post.
        /// </summary>
        public int UserId { get; set; }
    }
}