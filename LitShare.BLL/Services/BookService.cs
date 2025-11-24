// <copyright file="BookService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace LitShare.BLL.Services
{
    using LitShare.BLL.DTOs;
    using LitShare.DAL;
    using LitShare.DAL.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides services for retrieving, searching, and filtering book/post listings.
    /// </summary>
    public class BookService
    {
        private readonly LitShareDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookService"/> class.
        /// </summary>
        /// <param name="context">The database context (optional, defaults to a new instance if null).</param>
        public BookService(LitShareDbContext? context = null)
        {
            this.context = context ?? new LitShareDbContext();
        }

        /// <summary>
        /// Retrieves a list of all book listings from the database.
        /// </summary>
        /// <returns>A list of <see cref="BookDto"/> objects.</returns>
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
                        ImagePath = p.PhotoUrl,
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
        /// Retrieves a list of all unique genres from the database.
        /// </summary>
        /// <returns>A list of genre names.</returns>
        public async Task<List<string>> GetGenresAsync()
        {
            try
            {
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
                return await this.context.Genres
                    .AsNoTracking()
                    .OrderBy(g => g.Name)
                    .Select(g => g.Name)
                    .ToListAsync();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні жанрів: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Filters a given list of books based on search criteria, location, deal type, and genres.
        /// </summary>
        /// <param name="books">The initial list of books to filter.</param>
        /// <param name="search">Text to search in Title or Author.</param>
        /// <param name="location">Location filter.</param>
        /// <param name="dealType">Deal type filter.</param>
        /// <param name="genres">List of genres to filter by.</param>
        /// <returns>A filtered list of <see cref="BookDto"/> objects.</returns>
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
        /// Retrieves book listings associated with a specific user ID.
        /// </summary>
        /// <param name="userId">The ID of the user whose books to retrieve.</param>
        /// <returns>A list of <see cref="BookDto"/> objects.</returns>
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
                        ImagePath = p.PhotoUrl,
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
        /// Retrieves a single book listing by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to retrieve.</param>
        /// <returns>A single <see cref="BookDto"/> object or null if not found.</returns>
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
                    ImagePath = p.PhotoUrl,
                    UserId = p.UserId,
                })
                .FirstOrDefaultAsync();
        }
    }
}