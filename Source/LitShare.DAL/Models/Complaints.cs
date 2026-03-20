namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("complaints")]
    public class Complaints
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("text")]
        public string? Text { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("post_id")]
        [ForeignKey("Post")]
        public int PostId { get; set; }

        [Column("complainant_id")]
        [ForeignKey("Complainant")]
        public int ComplainantId { get; set; }

        public virtual Posts? Post { get; set; }

        public virtual Users? Complainant { get; set; }
    }
}