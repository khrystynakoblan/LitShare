// <copyright file="LitShareDbContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL
{
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// Represents the Entity Framework Core database session for the LitShare application.
    /// This class manages database connectivity, configuration, and model mapping.
    /// </summary>
    public class LitShareDbContext : DbContext
    {
        private static bool _mapperConfigured = false;

        /// <summary>
        /// Gets or sets the DbSet for managing User entities.
        /// </summary>
        public DbSet<Users> Users { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for managing Post entities.
        /// </summary>
        public DbSet<Posts> Posts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for managing Genre entities.
        /// </summary>
        public DbSet<Genres> Genres { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for managing Complaint entities.
        /// </summary>
        public DbSet<Complaints> Complaints { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for managing the BookGenres join entities.
        /// </summary>
        public DbSet<BookGenres> BookGenres { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// Used for manual creation or local configuration fallback.
        /// </summary>
        public LitShareDbContext()
        {
            this.ConfigureMapper();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LitShareDbContext"/> class.
        /// Used for Dependency Injection (DI) with provided options.
        /// </summary>
        /// <param name="options">The options for this context.</param>
        public LitShareDbContext(DbContextOptions<LitShareDbContext> options)
            : base(options)
        {
            this.ConfigureMapper();
        }

        /// <summary>
        /// Configures the PostgreSQL Npgsql timestamp legacy switch if not already set.
        /// </summary>
        private void ConfigureMapper()
        {
            if (_mapperConfigured) return;
            _mapperConfigured = true;

            // Ensures compatibility with older PostgreSQL DateTime types (Npgsql V6/V7 requirement)
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /// <summary>
        /// Configures the database connection options if options were not passed via the constructor.
        /// This is typically used for local testing or migrations.
        /// </summary>
        /// <param name="optionsBuilder">The builder used to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // NOTE: Hardcoded connection string is visible here. For production, use configuration/secrets.
                string connectionString =
                    "User Id=postgres.arrxdcvkamsqxudjxvkm;Password=i9n4Nf?aAq#gT!N;Server=aws-1-eu-west-3.pooler.supabase.com;Port=5432;Database=postgres";

                optionsBuilder.UseNpgsql(connectionString, o =>
                {
                    // Adds retry logic for transient connection failures
                    o.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    // Maps C# enums to custom PostgreSQL types (required by Npgsql)
                    o.MapEnum<DealType>("deal_type_t");
                    o.MapEnum<RoleType>("role_t");
                });
            }
        }

        /// <summary>
        /// Configures the model schema, relationships, and PostgreSQL-specific features.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Creates the custom enum types in PostgreSQL if they don't exist
            modelBuilder.HasPostgresEnum<DealType>("deal_type_t");
            modelBuilder.HasPostgresEnum<RoleType>("role_t");

            // Explicitly set the column type for the Role property
            modelBuilder.Entity<Users>()
                .Property(u => u.Role)
                .HasColumnType("role_t");

            // Defines the composite primary key for the many-to-many join table
            modelBuilder.Entity<BookGenres>()
                .HasKey(bg => new { bg.PostId, bg.GenreId });
        }
    }
}