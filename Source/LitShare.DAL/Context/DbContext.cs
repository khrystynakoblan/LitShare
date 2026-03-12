// <copyright file="LitShareDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Context
{
    using System;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    /// <summary>
    /// Represents the Entity Framework Core database session for the LitShare application.
    /// </summary>
    public class LitShareDbContext : DbContext
    {
        private static bool mapperConfigured = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// </summary>
        public LitShareDbContext() => ConfigureMapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// </summary>
        /// <param name="options">The options for this context.</param>
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");
            modelBuilder.HasPostgresEnum<RoleType>("role_t");

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(u => u.Role)
                      .HasColumnType("role_t");
            });

            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.PostId, bg.GenreId });

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Глобальні налаштування драйвера Npgsql.
        /// </summary>
        private static void ConfigureMapper()
        {
            if (mapperConfigured) return;

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            mapperConfigured = true;
        }
    }
}