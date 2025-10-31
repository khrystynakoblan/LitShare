using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("complaints")]
    public class Complaints
    {
        [Key]
        public int id { get; set; }

        public string text { get; set; }

        [Column("date")]
        public DateTime date { get; set; }

        [Column("post_id")]
        public int post_id { get; set; }

        [Column("complainant_id")]
        public int complainant_id { get; set; }
    }
}