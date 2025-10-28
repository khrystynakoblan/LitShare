using LitShare.DAL.Models; 
using Microsoft.EntityFrameworkCore;

namespace LitShare.DAL
{
    public class LitShareDbContext : DbContext
    {
        // "Набори" даних, які представляють ваші таблиці
        public DbSet<Users> Users { get; set; }
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<Complaints> Complaints { get; set; }
        public DbSet<BookGenres> BookGenres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // РЯДОК ПІДКЛЮЧЕННЯ З SUPABASE
            string connectionString = "User Id=postgres.arrxdcvkamsqxudjxvkm;Password=QioEm2I5SBGYHjs7;Server=aws-1-eu-west-3.pooler.supabase.com;Port=6543;Database=postgres";

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // вказуємо EF Core, що таблиця 'BookGenre' має складений первинний ключ (з двох колонок)
            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.post_id, bg.genre_id });
        }
    }
}