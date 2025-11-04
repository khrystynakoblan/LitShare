using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("books_genres")]
    public class BookGenres
    {
        [Column("post_id")]
        public int post_id { get; set; }

        [Column("genre_id")]
        public int genre_id { get; set; }

        [ForeignKey("post_id")]
        public virtual Posts Post { get; set; }

        [ForeignKey("genre_id")]
        public virtual Genres Genre { get; set; }
    }
}