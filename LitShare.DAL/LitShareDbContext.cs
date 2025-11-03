using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LitShare.DAL
{
    public class LitShareDbContext : DbContext
    {
        static LitShareDbContext()
        {
            // ВАЖЛИВО: Мапінг ПЕРЕД будь-яким використанням
            NpgsqlConnection.GlobalTypeMapper.MapEnum<DealType>("deal_type_t");
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Posts> posts { get; set; }
        public DbSet<Genres> genres { get; set; }
        public DbSet<Complaints> complaints { get; set; }
        public DbSet<BookGenres> bookGenres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "User Id=postgres.arrxdcvkamsqxudjxvkm;Password=i9n4Nf?aAq#gT!N;Server=aws-1-eu-west-3.pooler.supabase.com;Port=5432;Database=postgres";

            optionsBuilder.UseNpgsql(connectionString, o => o.MapEnum<DealType>("deal_type_t"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");

            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.post_id, bg.genre_id });
        }
    }
}