using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("posts")]
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Column("user_id")] // Явно вказуємо ім'я стовпця
        public int UserId { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }

        [Column("deal_type")]
        public string DealType { get; set; } // Ваш скрипт використовує 'deal_type_t'

        public string Description { get; set; }

        [Column("photo_url")]
        public string? PhotoUrl { get; set; } // Може бути null
    }
}