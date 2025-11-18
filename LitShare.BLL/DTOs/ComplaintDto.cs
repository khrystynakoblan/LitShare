// -----------------------------------------------------------------------
// <copyright file="ComplaintDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.BLL.DTOs
{
    using System; // Потрібно для DateTime

    /// <summary>
    /// Представляє об'єкт передачі даних (DTO) для детальної інформації про скаргу.
    /// Включає дані про текст скарги, книгу та скаржника.
    /// </summary>
    public class ComplaintDto
    {
        /// <summary>
        /// Gets or sets the text content of the complaint.
        /// </summary>
        public string Text { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the title of the book/post the complaint is filed against.
        /// </summary>
        public string BookTitle { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the name of the user who filed the complaint.
        /// </summary>
        public string UserName { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the date and time the complaint was filed.
        /// </summary>
        public DateTime Date { get; set; }
    }
}