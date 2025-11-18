// -----------------------------------------------------------------------
// <copyright file="DealType.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using NpgsqlTypes;

    /// <summary>
    /// Визначає можливі типи угод для публікацій у системі.
    /// </summary>
    public enum DealType
    {
        /// <summary>
        /// Угода обміну товарами (книга на книгу).
        /// </summary>
        [PgName("exchange")]
        Exchange,

        /// <summary>
        /// Угода дарування (безкоштовна передача).
        /// </summary>
        [PgName("donation")]
        Donation,
    }
}