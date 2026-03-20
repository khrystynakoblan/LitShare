namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("books_genres")]
    public class BookGenres
    {
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }

        [ForeignKey("PostId")]
        public virtual Posts? Post { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genres? Genre { get; set; }
    }
}