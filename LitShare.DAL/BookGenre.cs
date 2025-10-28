using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("books_genres")]
    public class BookGenre
    {
        // У цієї таблиці немає одного 'Id', у неї складений ключ
        [Column("post_id")]
        public int PostId { get; set; }

        [Column("genre_id")]
        public int GenreId { get; set; }
    }
}