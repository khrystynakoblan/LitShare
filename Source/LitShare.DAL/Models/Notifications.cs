using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("notifications")]
    public class Notifications
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("message")]
        public string Message { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_sent")]
        public bool IsSent { get; set; } = false;

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        public virtual Users? User { get; set; }
    }
}