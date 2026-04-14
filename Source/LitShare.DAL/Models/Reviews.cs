namespace LitShare.DAL.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("reviews")]
    public class Reviews
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("text")]
        public string? Text { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("reviewer_id")]
        public int ReviewerId { get; set; }

        [Column("reviewed_user_id")]
        public int ReviewedUserId { get; set; }

        [ForeignKey("ReviewerId")]
        public virtual Users? Reviewer { get; set; }

        [ForeignKey("ReviewedUserId")]
        public virtual Users? ReviewedUser { get; set; }
    }
}