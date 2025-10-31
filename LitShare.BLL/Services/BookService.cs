using LitShare.DAL;
using LitShare.BLL.DTOs;

namespace LitShare.BLL.Services
{
    public class BookService
    {
        public List<BookDto> GetAllBooks()
        {
            using (var db = new LitShareDbContext())
            {
                var postsWithGenres = (from p in db.posts
                                       join bg in db.bookGenres on p.id equals bg.post_id
                                       join g in db.genres on bg.genre_id equals g.id
                                       join u in db.Users on p.user_id equals u.id
                                       select new
                                       {
                                           PostId = p.id,
                                           Title = p.title,
                                           Author = p.author,
                                           Genre = g.name,
                                           DealType = p.deal_type == "exchange" ? "Обмін" : "Безкоштовно",
                                           Location = u.city,
                                           ImagePath = p.photo_url
                                       }).ToList();

                return postsWithGenres
                    .GroupBy(x => x.PostId)
                    .Select(g => new BookDto
                    {
                        Title = g.First().Title,
                        Author = g.First().Author,
                        Location = g.First().Location,
                        Genre = string.Join(", ", g.Select(x => x.Genre)),
                        DealType = g.First().DealType,
                        ImagePath = g.First().ImagePath
                    })
                    .ToList();
            }
        }

        public List<string> GetGenres()
        {
            using (var db = new LitShareDbContext())
            {
                return db.genres.Select(g => g.name).ToList();
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
    }
}
