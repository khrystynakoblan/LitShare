// -----------------------------------------------------------------------
// <copyright file="BookDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.BLL.DTOs
{
    /// <summary>
    /// Представляє об'єкт передачі даних (DTO) для інформації про книгу/публікацію.
    /// Використовується для передачі даних між BLL та Presentation.
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the publication.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        public string Title { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public string Author { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the description of the book/post.
        /// </summary>
        public string Description { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the main genre of the book.
        /// </summary>
        public string Genre { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the type of deal (Exchange, Donation, etc.).
        /// </summary>
        public string DealType { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the geographical location of the user/post.
        /// </summary>
        public string Location { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the path or URL to the image associated with the book.
        /// </summary>
        public string ImagePath { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the identifier of the user who owns the book/post.
        /// </summary>
        public int UserId { get; set; }
    }
}