using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("genres")]
    public class Genres
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
    }
}