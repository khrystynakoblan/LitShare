namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("posts")]
    public class Posts
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("title")]
        public string? Title { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("author")]
        public string? Author { get; set; }

        [Column("deal_type")]
        public DealType DealType { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        public virtual ICollection<BookGenres> BookGenres { get; set; } = new List<BookGenres>();

        public virtual ICollection<Complaints>? Complaints { get; set; }

        public virtual ICollection<Favorites>? FavoritedBy { get; set; }
    }
}