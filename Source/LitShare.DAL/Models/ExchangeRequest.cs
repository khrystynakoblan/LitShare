using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("exchange_requests")]
    public class ExchangeRequest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("sender_id")]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        public virtual Users Sender { get; set; } = null!;

        [Required]
        [Column("post_id")]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public virtual Posts Post { get; set; } = null!;

        [Column("status")]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}