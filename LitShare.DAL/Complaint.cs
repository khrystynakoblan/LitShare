using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("complaints")]
    public class Complaint
    {
        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        [Column("post_id")]
        public int PostId { get; set; }

        [Column("complainant_id")]
        public int ComplainantId { get; set; }
    }
}