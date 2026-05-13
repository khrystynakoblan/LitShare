namespace LitShare.DAL.Context
{
    using System;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    public class LitShareDbContext : DbContext
    {
        private static bool mapperConfigured = false;

        public LitShareDbContext()
        {
            ConfigureMapper();
        }

        public LitShareDbContext(DbContextOptions<LitShareDbContext> options)
            : base(options)
        {
            ConfigureMapper();
        }

        public DbSet<Users> Users { get; set; }

        public DbSet<Posts> Posts { get; set; }

        public DbSet<Genres> Genres { get; set; }

        public DbSet<Complaints> Complaints { get; set; }

        public DbSet<BookGenres> BookGenres { get; set; }

        public DbSet<Reviews> Reviews { get; set; }

        public DbSet<Favorites> Favorites { get; set; }

        public DbSet<ExchangeRequest> ExchangeRequests { get; set; }

        public DbSet<Notifications> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");
            modelBuilder.HasPostgresEnum<RoleType>("role_t");

            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Users>().Property(u => u.Role).HasDefaultValue(RoleType.User);
            modelBuilder.Entity<Complaints>().Property(c => c.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Posts>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Complaints>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Complaints)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Complaints>()
                .HasOne(c => c.Complainant)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.ComplainantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookGenres>(entity =>
            {
                entity.HasKey(bg => new { bg.PostId, bg.GenreId });

                entity.HasOne(bg => bg.Post)
                      .WithMany(p => p.BookGenres)
                      .HasForeignKey(bg => bg.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bg => bg.Genre)
                      .WithMany(g => g.BookGenres)
                      .HasForeignKey(bg => bg.GenreId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Users>()
                .Property(u => u.Role)
                .HasColumnType("role_t");

            modelBuilder.Entity<Posts>()
                .Property(p => p.DealType)
                .HasColumnType("deal_type_t");

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.HasKey(f => new { f.UserId, f.PostId });

                entity.HasOne(f => f.User)
                      .WithMany(u => u.Favorites)
                      .HasForeignKey(f => f.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Post)
                      .WithMany(p => p.FavoritedBy)
                      .HasForeignKey(f => f.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Reviews>().Property(r => r.Date).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reviews>()
                .HasOne(r => r.ReviewedUser)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(r => r.ReviewedUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reviews>()
                .HasIndex(r => new { r.ReviewerId, r.ReviewedUserId })
                .IsUnique();

            modelBuilder.HasPostgresEnum<RequestStatus>();

            modelBuilder.Entity<ExchangeRequest>()
                .HasIndex(e => new { e.SenderId, e.PostId })
                .IsUnique();

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany()
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(n => n.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }

        /// <summary>
        /// Configures global settings for the Npgsql driver.
        /// </summary>
        private static void ConfigureMapper()
        {
            if (mapperConfigured)
            {
                return;
            }

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            mapperConfigured = true;
        }
    }
}