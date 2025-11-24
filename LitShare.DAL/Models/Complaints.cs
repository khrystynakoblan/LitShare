// <copyright file="Complaints.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a user complaint filed against a specific post.
    /// </summary>
    [Table("complaints")]
    public class Complaints
    {
        /// <summary>
        /// Gets or sets the unique identifier (primary key) of the complaint.
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the detailed text or message content of the complaint.
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the complaint was filed.
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the post being complained about.
        /// </summary>
        [Column("post_id")]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        /// <summary>
        /// Gets or sets the foreign key identifier for the user who filed the complaint.
        /// </summary>
        [Column("complainant_id")]
        [ForeignKey("Complainant")]
        public int ComplainantId { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the related Post entity.
        /// </summary>
        public virtual Posts? Post { get; set; }

        /// <summary>
        /// Gets or sets the navigation property for the User who filed the complaint.
        /// </summary>
        public virtual Users? Complainant { get; set; }
    }
}