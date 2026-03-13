// <copyright file="RoleType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    /// <summary>
    /// Defines the possible roles a user can have within the application.
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// Standard application user role. Maps to 'user' in the database.
        /// </summary>
        [PgName("user")]
        User,

        /// <summary>
        /// Administrative user role with elevated privileges. Maps to 'admin' in the database.
        /// </summary>
        [PgName("admin")]
        Admin,
    }
}