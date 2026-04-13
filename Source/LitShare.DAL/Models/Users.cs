namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("users")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("name")]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("password_hash")]
        public string? PasswordHash { get; set; }

        [Column("about")]
        public string? About { get; set; }

        [MaxLength(100)]
        [Column("region")]
        public string? Region { get; set; }

        [MaxLength(100)]
        [Column("district")]
        public string? District { get; set; }

        [MaxLength(100)]
        [Column("city")]
        public string? City { get; set; }

        [Column("role")]
        public RoleType Role { get; set; }

        [Column("photo_url")]
        public string? PhotoUrl { get; set; }

        public ICollection<Posts>? Posts { get; set; }

        public ICollection<Complaints>? Complaints { get; set; }

        public ICollection<Reviews>? ReviewsGiven { get; set; }

        public ICollection<Reviews>? ReviewsReceived { get; set; }
    }
}