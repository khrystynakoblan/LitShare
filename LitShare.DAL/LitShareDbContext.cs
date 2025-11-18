// -----------------------------------------------------------------------
// <copyright file="LitShareDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL
{
    using System;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Контекст бази даних для роботи з Entity Framework Core.
    /// </summary>
    public class LitShareDbContext : DbContext
    {
        private static bool mapperConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// </summary>
        public LitShareDbContext()
        {
            this.ConfigureMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public LitShareDbContext(DbContextOptions<LitShareDbContext> options)
            : base(options)
        {
            this.ConfigureMapper();
        }

        /// <summary>
        /// Gets or sets the collection of Users entities.
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Gets or sets the collection of Posts entities.
        /// </summary>
        public DbSet<Posts> Posts { get; set; }

        /// <summary>
        /// Gets or sets the collection of Genres entities.
        /// </summary>
        public DbSet<Genres> Genres { get; set; }

        /// <summary>
        /// Gets or sets the collection of Complaints entities.
        /// </summary>
        public DbSet<Complaints> Complaints { get; set; }

        /// <summary>
        /// Gets or sets the collection of BookGenres join entities.
        /// </summary>
        public DbSet<BookGenres> BookGenres { get; set; }

        /// <inheritdoc/>
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
                });
            }
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");

            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.PostId, bg.GenreId });

            base.OnModelCreating(modelBuilder);
        }

        private void ConfigureMapper()
        {
            if (mapperConfigured)
            {
                return;
            }

            mapperConfigured = true;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}