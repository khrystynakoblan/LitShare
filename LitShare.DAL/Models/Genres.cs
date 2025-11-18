// -----------------------------------------------------------------------
// <copyright file="Genres.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.DAL.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Представляє модель даних для жанрів (наприклад, жанрів книг).
    /// </summary>
    [Table("genres")]
    public class Genres
    {
        /// <summary>
        /// Gets or sets the unique identifier of the genre.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre. This field is required.
        /// </summary>
#pragma warning disable SA1206 // Declaration keywords should follow order
        public required string Name { get; set; }
#pragma warning restore SA1206 // Declaration keywords should follow order
    }
}