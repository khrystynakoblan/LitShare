// -----------------------------------------------------------------------
// <copyright file="Complaints.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє модель даних для скарг, пов'язаних з публікаціями.
    /// </summary>
    [Table("complaints")]
    public class Complaints
    {
        /// <summary>
        /// Gets or sets the unique identifier of the complaint.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the detailed text description of the complaint.
        /// </summary>
        public string Text { get; set; } = string.Empty; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the date and time when the complaint was submitted.
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the related post.
        /// </summary>
        [Column("post_id")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the user who filed the complaint.
        /// </summary>
        [Column("complainant_id")]
        public int ComplainantId { get; set; }

        /// <summary>
        /// Gets or sets the virtual navigation property to the post being complained about.
        /// </summary>
        public virtual Posts Post { get; set; } = null!; // Виправляє CS8618

        /// <summary>
        /// Gets or sets the virtual navigation property to the user who filed the complaint.
        /// </summary>
        public virtual Users Complainant { get; set; } = null!; // Виправляє CS8618
    }
}