using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace LitShare.DAL
{
    public class LitShareDbContext : DbContext
    {
        private static bool _mapperConfigured = false;




        public DbSet<Users> Users { get; set; }
        public DbSet<Posts> posts { get; set; }
        public DbSet<Genres> genres { get; set; }
        public DbSet<Complaints> complaints { get; set; }
        public DbSet<BookGenres> bookGenres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            string connectionString = "User Id=postgres.arrxdcvkamsqxudjxvkm;Password=QioEm2I5SBGYHjs7;Server=aws-1-eu-west-3.pooler.supabase.com;Port=6543;Database=postgres";

            optionsBuilder.UseNpgsql(connectionString);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.post_id, bg.genre_id });
        }
    }
}