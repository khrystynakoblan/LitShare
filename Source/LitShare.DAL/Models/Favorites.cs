namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("favorites")]
    public class Favorites
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [ForeignKey("PostId")]
        public virtual Posts? Post { get; set; }
    }
}