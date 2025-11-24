using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;

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

        public LitShareDbContext()
        {
            ConfigureMapper();
        }

        public LitShareDbContext(DbContextOptions<LitShareDbContext> options)
            : base(options)
        {
            ConfigureMapper();
        }

        private void ConfigureMapper()
        {
            if (_mapperConfigured) return;
            _mapperConfigured = true;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString =
                    "User Id=postgres.arrxdcvkamsqxudjxvkm;Password=i9n4Nf?aAq#gT!N;Server=aws-1-eu-west-3.pooler.supabase.com;Port=5432;Database=postgres";

                optionsBuilder.UseNpgsql(connectionString, o =>
                {
                    o.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);
                    o.MapEnum<DealType>("deal_type_t");
                    o.MapEnum<RoleType>("role_t");
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");
            modelBuilder.HasPostgresEnum<RoleType>("role_t");

            modelBuilder.Entity<Users>()
                .Property(u => u.role)
                .HasColumnType("role_t");


            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.post_id, bg.genre_id });
        }
    }
}