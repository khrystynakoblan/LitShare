// <copyright file="DealType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    /// <summary>
    /// Specifies the type of transaction or arrangement for sharing a book or post.
    /// </summary>
    public enum DealType
    {
        /// <summary>
        /// The item is available for exchange with another book or item.
        /// Maps to 'exchange' in the database.
        /// </summary>
        [PgName("exchange")]
        Exchange,

        /// <summary>
        /// The item is available as a free donation.
        /// Maps to 'donation' in the database.
        /// </summary>
        [PgName("donation")]
        Donation,
    }
}