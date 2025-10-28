using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("genres")]
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}