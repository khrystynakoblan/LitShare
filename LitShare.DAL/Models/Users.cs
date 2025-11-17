using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LitShare.DAL.Models
{
    [Table("users")]
    public class Users
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string? about { get; set; }

        public string region { get; set; }
        public string district { get; set; }
        public string city { get; set; }
        public string? photo_url { get; set; }
        public ICollection<Posts>? posts { get; set; }
    }
}