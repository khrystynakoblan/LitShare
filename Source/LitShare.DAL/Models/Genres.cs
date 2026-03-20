namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("genres")]
    public class Genres
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string? Name { get; set; }

        public virtual ICollection<BookGenres>? BookGenres { get; set; }
    }
}