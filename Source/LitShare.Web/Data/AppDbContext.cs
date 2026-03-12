// <copyright file="AppDbContext.cs" company="LitShare">
// Copyright (c) LitShare. All rights reserved.
// </copyright>

namespace LitShare.Web.Data
{
    using LitShare.Web.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context for the LitShare application.
    /// Handles interaction with the database using Entity Framework.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">Database configuration options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the users table.
        /// </summary>
        public DbSet<user> Users { get; set; }

        /// <summary>
        /// Gets or sets the posts table.
        /// </summary>
        public DbSet<post> Posts { get; set; }

        /// <summary>
        /// Gets or sets the genres table.
        /// </summary>
        public DbSet<genre> Genres { get; set; }

        /// <summary>
        /// Gets or sets the complaints table.
        /// </summary>
        public DbSet<complaint> Complaints { get; set; }
    }
}