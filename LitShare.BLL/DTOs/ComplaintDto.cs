// <copyright file="ComplaintDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.DTOs
{
    /// <summary>
    /// Represents the data necessary to display a single user complaint.
    /// </summary>
    public class ComplaintDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the complaint.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the detailed message text of the complaint.
        /// </summary>
        public string? Text { get; set; } // CS8618 Fix

        /// <summary>
        /// Gets or sets the title of the book/post the complaint is filed against.
        /// </summary>
        public string? BookTitle { get; set; } // CS8618 Fix

        /// <summary>
        /// Gets or sets the name of the user who filed the complaint.
        /// </summary>
        public string? UserName { get; set; } // CS8618 Fix

        /// <summary>
        /// Gets or sets the date and time the complaint was filed.
        /// </summary>
        public DateTime Date { get; set; }
    }
}