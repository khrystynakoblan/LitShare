using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("books_genres")]
    public class BookGenres
    {
        // У цієї таблиці немає одного 'Id', у неї складений ключ
        [Column("post_id")]
        public int post_id { get; set; }

        [Column("genre_id")]
        public int genre_id { get; set; }
    }
}