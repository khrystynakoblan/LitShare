using LitShare.BLL.Services;
using LitShare.DAL;
using LitShare.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Threading.Tasks;

namespace LitShare.Tests
{
    public class ServicesTests
    {
        private LitShareDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LitShareDbContext(options);
        }

        [Fact]
        public async Task AddUser_ShouldAddUser()
        {
            using var db = GetInMemoryDb();
            var usersService = new UserService(db);

            usersService.AddUser("Test User", "test@example.com", "12345", "pass", "RegionX", "DistrictX", "CityX");

            var savedUser = await db.Users.FirstOrDefaultAsync(u => u.email == "test@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test User", savedUser.name);
            Assert.Equal("12345", savedUser.phone);

            // Перевіряємо пароль через BCrypt
            Assert.True(BCrypt.Net.BCrypt.Verify("pass", savedUser.password));
        }

        [Fact]
        public async Task ValidateUser_ShouldReturnTrue_WhenCredentialsCorrect()
        {
            using var db = GetInMemoryDb();
            var usersService = new UserService(db);

            usersService.AddUser("Test", "a@b.com", "111111", "123", "X", "X", "X");

            bool result = await usersService.ValidateUser("a@b.com", "123");
            Assert.True(result);
        }

        [Fact]
        public async Task AddComplaint_ShouldAddComplaint()
        {
            using var db = GetInMemoryDb();
            var usersService = new UserService(db);
            var complaintsService = new ComplaintsService(db);

            usersService.AddUser("User", "u@u.com", "123", "123", "X", "X", "X");
            var userId = (await db.Users.FirstAsync()).id;

            db.posts.Add(new Posts
            {
                id = 1,
                title = "Book",
                author = "Author",
                user_id = userId,
                deal_type = DealType.Donation,
                description = "Desc",
                User = await db.Users.FirstAsync()
            });
            await db.SaveChangesAsync();

            complaintsService.AddComplaint("Test complaint", 1, userId);

            var saved = await db.complaints.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("Test complaint", saved.text);
            Assert.Equal(1, saved.post_id);
            Assert.Equal(userId, saved.complainant_id);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnBooks()
        {
            using var db = GetInMemoryDb();
            var usersService = new UserService(db);
            usersService.AddUser("User1", "u@u.com", "123", "123", "X", "X", "X");
            var user = await db.Users.FirstAsync();

            db.posts.Add(new Posts { id = 1, title = "Book1", author = "Author1", deal_type = DealType.Donation, user_id = user.id, User = user, description = "Desc" });
            db.posts.Add(new Posts { id = 2, title = "Book2", author = "Author2", deal_type = DealType.Exchange, user_id = user.id, User = user, description = "Desc" });
            await db.SaveChangesAsync();

            var booksService = new BookService(db);
            var allBooks = await booksService.GetAllBooksAsync();

            Assert.Equal(2, allBooks.Count);
        }

        [Fact]
        public async Task GetGenres_ShouldReturnDistinctGenres()
        {
            using var db = GetInMemoryDb();

            db.genres.Add(new Genres { id = 1, name = "Fantasy" });
            db.genres.Add(new Genres { id = 2, name = "Sci-Fi" });
            await db.SaveChangesAsync();

            var booksService = new BookService(db);
            var genres = await booksService.GetGenresAsync();

            Assert.Contains("Fantasy", genres);
            Assert.Contains("Sci-Fi", genres);
            Assert.Equal(2, genres.Count);
        }
    }
}
