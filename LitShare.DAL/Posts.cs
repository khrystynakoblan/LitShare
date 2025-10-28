using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("posts")]
    public class Posts
    {
        [Key]
        public int id { get; set; }

        [Column("user_id")] 
        public int user_id { get; set; }

        public string title { get; set; }
        public string author { get; set; }

        [Column("deal_type")]
        public string deal_type { get; set; } // Ваш скрипт використовує 'deal_type_t'

        public string description { get; set; }

        [Column("photo_url")]
        public string? photo_url { get; set; } // Може бути null
    }
}