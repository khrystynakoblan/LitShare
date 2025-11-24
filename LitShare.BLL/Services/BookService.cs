using LitShare.DAL;
using LitShare.BLL.DTOs;
using Microsoft.EntityFrameworkCore;
using LitShare.DAL.Models;

namespace LitShare.BLL.Services
{
    public class BookService
    {
        private readonly LitShareDbContext _context;

                public BookService(LitShareDbContext? context = null)
        {
            _context = context ?? new LitShareDbContext();
        }

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            try
            {
                var books = await _context.posts
                    .Include(p => p.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                    .Include(p => p.User)
                    .AsNoTracking()
                    .Select(p => new BookDto
                    {
                        Id = p.id,
                        Title = p.title,
                        Author = p.author,
                        Location = p.User.city,
                        Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.name)),
                        DealType = p.deal_type == DealType.Exchange ? "Обмін" : "Безкоштовно",
                        ImagePath = p.photo_url
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

        public async Task<List<string>> GetGenresAsync()
        {
            try
            {
                return await _context.genres
                    .AsNoTracking()
                    .OrderBy(g => g.name)
                    .Select(g => g.name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при завантаженні жанрів: {ex.Message}");
                return new List<string>();
            }
        }

        public List<BookDto> GetFilteredBooks(List<BookDto> books, string? search, string? location, string? dealType, List<string> genres)
        {
            var filtered = books.AsEnumerable();

            if (!string.IsNullOrEmpty(location))
                filtered = filtered.Where(b => b.Location?.Contains(location, StringComparison.OrdinalIgnoreCase) == true);

            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(b =>
                    (b.Title?.Contains(search, StringComparison.OrdinalIgnoreCase) == true) ||
                    (b.Author?.Contains(search, StringComparison.OrdinalIgnoreCase) == true));

            if (!string.IsNullOrEmpty(dealType))
                filtered = filtered.Where(b => b.DealType == dealType);

            if (genres?.Any() == true)
                filtered = filtered.Where(b => genres.Any(g =>
                    b.Genre?.Contains(g, StringComparison.OrdinalIgnoreCase) == true));

            return filtered.ToList();
        }

        public async Task<List<BookDto>> GetBooksByUserIdAsync(int userId)
        {
            try
            {
                var books = await _context.posts
                    .Where(p => p.user_id == userId)
                    .Include(p => p.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                    .Include(p => p.User)
                    .AsNoTracking()
                    .Select(p => new BookDto
                    {
                        Id = p.id,
                        Title = p.title,
                        Author = p.author,
                        Location = p.User.city,
                        Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.name)),
                        DealType = p.deal_type == DealType.Exchange ? "Обмін" : "Безкоштовно",
                        ImagePath = p.photo_url
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

        public async Task<BookDto?> GetBookById(int id)
        {
            return await _context.posts
                .Include(p => p.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(p => p.User)
                .AsNoTracking()
                .Where(p => p.id == id)
                .Select(p => new BookDto
                {
                    Id = p.id,
                    Title = p.title,
                    Author = p.author,
                    Location = p.User.city,
                    Genre = string.Join(", ", p.BookGenres.Select(bg => bg.Genre.name)),
                    DealType = p.deal_type == DealType.Exchange ? "Обмін" : "Безкоштовно",
                    ImagePath = p.photo_url,
                    UserId = p.user_id
                })
                .FirstOrDefaultAsync();
        }
    }
}