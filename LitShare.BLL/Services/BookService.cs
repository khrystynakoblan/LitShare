// -----------------------------------------------------------------------
// <copyright file="BookService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace LitShare.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.DTOs;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Надає бізнес-логіку для керування публікаціями книг (Posts).
    /// </summary>
    public class BookService
    {
        private readonly LitShareDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="context">The optional database context. If null, a new context is created.</param>
        public BookService(LitShareDbContext? context = null)
        {
            this.context = context ?? new LitShareDbContext();
        }

        /// <summary>
        /// Asynchronously retrieves a list of all books, mapped to <see cref="BookDto"/>.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of books.</returns>
        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            try
            {
                var books = await this.context.Posts
                    .Include(p => p.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                    .Include(p => p.User)
                    .AsNoTracking()
                    .Select(p => new BookDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Author = p.Author,
                        Location = p.User.City,
                        Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.Name)),
                        DealType = p.DealType == DealType.Exchange ? "Обмін" : "Безкоштовно",
                        ImagePath = p.PhotoUrl ?? string.Empty,
                        UserId = p.UserId,
                    })
                    .ToListAsync();

                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні книг: {ex.Message}");
                return new List<BookDto>();
            }
        }

        /// <summary>
        /// Asynchronously retrieves a list of all unique genre names.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of genres.</returns>
        public async Task<List<string>> GetGenresAsync()
        {
            try
            {
                return await this.context.Genres
                    .AsNoTracking()
                    .OrderBy(g => g.Name)
                    .Select(g => g.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні жанрів: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Filters a list of books based on various criteria (search, location, deal type, genres).
        /// </summary>
        /// <param name="books">The initial list of books to filter.</param>
        /// <param name="search">Optional search term for title or author.</param>
        /// <param name="location">Optional location filter.</param>
        /// <param name="dealType">Optional deal type filter.</param>
        /// <param name="genres">List of genres to filter by.</param>
        /// <returns>The filtered list of <see cref="BookDto"/> objects.</returns>
        public List<BookDto> GetFilteredBooks(List<BookDto> books, string? search, string? location, string? dealType, List<string> genres)
        {
            var filtered = books.AsEnumerable();

            if (!string.IsNullOrEmpty(location))
            {
                filtered = filtered.Where(b => b.Location?.Contains(location, StringComparison.OrdinalIgnoreCase) == true);
            }

            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(b =>
                    (b.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (b.Author?.Contains(search, StringComparison.OrdinalIgnoreCase) == true));
            }

            if (!string.IsNullOrEmpty(dealType))
            {
                filtered = filtered.Where(b => b.DealType == dealType);
            }

            if (genres?.Any() == true)
            {
                filtered = filtered.Where(b => genres.Any(g =>
                    b.Genre?.Contains(g, StringComparison.OrdinalIgnoreCase) == true));
            }

            return filtered.ToList();
        }

        /// <summary>
        /// Asynchronously retrieves a list of books posted by a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose books to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of books.</returns>
        public async Task<List<BookDto>> GetBooksByUserIdAsync(int userId)
        {
            try
            {
                var books = await this.context.Posts
                    .Where(p => p.UserId == userId)
                    .Include(p => p.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                    .Include(p => p.User)
                    .AsNoTracking()
                    .Select(p => new BookDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Author = p.Author,
                        Location = p.User.City,
                        Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.Name)),
                        DealType = p.DealType == DealType.Exchange ? "Обмін" : "Безкоштовно",
                        ImagePath = p.PhotoUrl ?? string.Empty,
                        UserId = p.UserId,
                    })
                    .ToListAsync();

                return books;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні книг користувача: {ex.Message}");
                return new List<BookDto>();
            }
        }

        /// <summary>
        /// Asynchronously retrieves a single book by its ID, mapped to <see cref="BookDto"/>.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the book DTO, or null if not found.</returns>
        public async Task<BookDto?> GetBookById(int id)
        {
            return await this.context.Posts
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(p => p.User)
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new BookDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Author = p.Author,
                    Location = p.User.City,
                    Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.Name)),
                    DealType = p.DealType == DealType.Exchange ? "Обмін" : "Безкоштовно",
                    ImagePath = p.PhotoUrl ?? string.Empty,
                    UserId = p.UserId,
                })
                .FirstOrDefaultAsync();
        }
    }
}