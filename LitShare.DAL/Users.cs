using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LitShare.DAL.Models
{
    [Table("users")] // Точно вказуємо назву таблиці
    public class Users
    {
        [Key] // Позначаємо первинний ключ
        public int id { get; set; }

        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string? about { get; set; } // '?' означає, що поле може бути null

        [Column("role")] // Вказуємо назву колонки, бо вона не збігається з 'Role'
        public string role { get; set; } // Ваш скрипт використовує 'role_t', але для C# це просто string

        public string region { get; set; }
        public string district { get; set; }
        public string city { get; set; }
    }
}