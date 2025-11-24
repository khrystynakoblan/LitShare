// <copyright file="Complaints.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a report or issue filed against a post.
    /// </summary>
    [Table("complaints")]
    public class Complaints
    {
        /// <summary>
        /// Gets or sets the unique identifier (primary key) of the complaint.
        /// </summary>
        [Key]
        [Column("id")] // PostgreSQL Fix: Явне мапування на нижній регістр
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the detailed message or description of the complaint.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets the date and time the complaint was filed.
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the post the complaint is against.
        /// </summary>
        [Column("post_id")]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier of the user who filed the complaint.
        /// </summary>
        [Column("complainant_id")]
        [ForeignKey("Complainant")]
        public int ComplainantId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the related Post entity.
        /// </summary>
        public virtual Posts? Post { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the user who filed the complaint.
        /// </summary>
        public virtual Users? Complainant { get; set; }
    }
}