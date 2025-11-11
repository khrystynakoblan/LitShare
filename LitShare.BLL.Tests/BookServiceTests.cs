using LitShare.BLL.Services;
using LitShare.BLL.DTOs;
using LitShare.DAL.Models;
using LitShare.DAL;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitShare.BLL.Tests
{
    public class BookServiceTests
    {
        private LitShareDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LitShareDbContext(options);
        }

        [Fact]
        public async Task GetGenresAsync_Should_Return_Sorted_List()
        {
            var context = GetDbContext();
            context.genres.AddRange(
                new Genres { name = "Drama" },
                new Genres { name = "Action" },
                new Genres { name = "Comedy" }
            );
            context.SaveChanges();

            var service = new BookService(context);

            var result = await service.GetGenresAsync();

            Assert.Equal(new[] { "Action", "Comedy", "Drama" }, result);
        }

        [Fact]
        public void GetFilteredBooks_Should_Filter_By_Genre_And_Location()
        {
            var books = new List<BookDto>
            {
                new() { Title = "A", Genre = "Action", Location = "Lviv", DealType = "Обмін" },
                new() { Title = "B", Genre = "Comedy", Location = "Kyiv", DealType = "Безкоштовно" },
                new() { Title = "C", Genre = "Action", Location = "Lviv", DealType = "Обмін" }
            };

            var service = new BookService(GetDbContext());

            var result = service.GetFilteredBooks(
                books,
                search: null,
                location: "Lviv",
                dealType: "Обмін",
                genres: new List<string> { "Action" });

            Assert.Equal(2, result.Count);
            Assert.All(result, b =>
            {
                Assert.Equal("Lviv", b.Location);
                Assert.Equal("Обмін", b.DealType);
                Assert.Contains("Action", b.Genre);
            });
        }

        [Fact]
        public async Task GetBooksByUserIdAsync_Should_Return_User_Books()
        {
            var context = GetDbContext();

            var user = new Users
            {
                name = "Test",
                email = "a@b.c",
                phone = "1",
                password = "x",
                region = "R",
                district = "D",
                city = "Lviv"
            };
            var genre = new Genres { name = "Drama" };
            context.Users.Add(user);
            context.genres.Add(genre);
            context.SaveChanges();

            var post = new Posts
            {
                user_id = user.id,
                title = "Book1",
                author = "Author",
                deal_type = DealType.Exchange,
                description = "Nice",
                BookGenres = new List<BookGenres> { new() { genre_id = genre.id, Genre = genre } },
                User = user
            };
            context.posts.Add(post);
            context.SaveChanges();

            var service = new BookService(context);

            var books = await service.GetBooksByUserIdAsync(user.id);

            Assert.Single(books);
            Assert.Equal("Book1", books[0].Title);
            Assert.Equal("Обмін", books[0].DealType);
        }

        [Fact]
        public async Task GetBookById_Should_Return_Correct_Book()
        {
            var context = GetDbContext();
            var user = new Users
            {
                name = "U",
                email = "x@y.com",
                phone = "1",
                password = "x",
                region = "R",
                district = "D",
                city = "Kyiv"
            };
            var genre = new Genres { name = "Action" };
            context.Users.Add(user);
            context.genres.Add(genre);
            context.SaveChanges();

            var post = new Posts
            {
                user_id = user.id,
                title = "TestBook",
                author = "Hero",
                deal_type = DealType.Donation,
                description = "Fine",
                BookGenres = new List<BookGenres> { new() { genre_id = genre.id, Genre = genre } },
                User = user
            };
            context.posts.Add(post);
            context.SaveChanges();

            var service = new BookService(context);

            var result = await service.GetBookById(post.id);

            Assert.NotNull(result);
            Assert.Equal("TestBook", result.Title);
            Assert.Equal("Безкоштовно", result.DealType);
            Assert.Equal("Kyiv", result.Location);
        }

        [Fact]
        public async Task GetAllBooksAsync_Should_Return_All_Books()
        {
            var context = GetDbContext();

            var user = new Users { name = "Reader", email = "r@r.com", phone = "1", password = "x", region = "R", district = "D", city = "Lviv" };
            var genre = new Genres { name = "Science" };
            context.Users.Add(user);
            context.genres.Add(genre);
            context.posts.Add(new Posts
            {
                user_id = user.id,
                title = "Book A",
                author = "Author A",
                deal_type = DealType.Exchange,
                description = "Desc",
                BookGenres = new List<BookGenres> { new() { Genre = genre, genre_id = genre.id } },
                User = user
            });
            context.SaveChanges();

            var service = new BookService(context);
            var result = await service.GetAllBooksAsync();

            Assert.Single(result);
            Assert.Equal("Обмін", result[0].DealType);
        }

        [Fact]
        public void GetFilteredBooks_Should_Filter_By_Search_And_DealType()
        {
            var books = new List<BookDto>
            {
                new() { Title = "The Great Book", Author = "John", Genre = "Drama", Location = "Kyiv", DealType = "Безкоштовно" },
                new() { Title = "Another", Author = "Jane", Genre = "Action", Location = "Kyiv", DealType = "Обмін" }
            };

            var service = new BookService(GetDbContext());
            var result = service.GetFilteredBooks(books, "Great", null, "Безкоштовно", new List<string>());

            Assert.Single(result);
            Assert.Equal("The Great Book", result[0].Title);
        }

        [Fact]
        public async Task GetBookById_Should_ReturnNull_When_NotFound()
        {
            var context = GetDbContext();
            var service = new BookService(context);

            var result = await service.GetBookById(9999);

            Assert.Null(result);
        }

        [Fact]
        public void GetFilteredBooks_Should_Return_Empty_When_No_Matches()
        {
            var books = new List<BookDto>
            {
                new() { Title = "A", Genre = "Drama", Location = "Kyiv", DealType = "Обмін" }
            };

            var service = new BookService(GetDbContext());
            var result = service.GetFilteredBooks(
                books, search: "X", location: "Lviv", dealType: "Безкоштовно", genres: new List<string> { "Action" });

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllBooksAsync_Should_Handle_Exception_And_Return_Empty_List()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new LitShareDbContext(options);
            await context.Database.EnsureDeletedAsync(); // видаляємо базу → згенерує помилку
            var service = new BookService(context);

            var result = await service.GetAllBooksAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void GetFilteredBooks_Should_Return_All_When_No_Filters()
        {
            var service = new BookService();
            var books = new List<BookDto>
            {
                new() { Title = "Гаррі Поттер", Author = "Роулінг", Location = "Львів", DealType = "Обмін", Genre = "Фентезі" },
                new() { Title = "Війна і мир", Author = "Толстой", Location = "Київ", DealType = "Безкоштовно", Genre = "Класика" }
            };

            var result = service.GetFilteredBooks(books, null, null, null, new List<string>());

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetFilteredBooks_Should_Return_Empty_When_No_Match()
        {
            var service = new BookService();
            var books = new List<BookDto>
            {
                new() { Title = "Гаррі Поттер", Author = "Роулінг", Location = "Львів", DealType = "Обмін", Genre = "Фентезі" }
            };

            var result = service.GetFilteredBooks(
                books,
                search: "Війна",
                location: "Київ",
                dealType: "Безкоштовно",
                genres: new List<string> { "Класика" }
            );

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBooksByUserIdAsync_Should_Handle_Exception_And_Return_Empty_List()
        {
            var brokenContext = new LitShareDbContext(
                new DbContextOptionsBuilder<LitShareDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options);

            var service = new BookService(brokenContext);
            await brokenContext.Database.EnsureDeletedAsync();

            var result = await service.GetBooksByUserIdAsync(999);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
       
        [Fact]
     
        public async Task GetBooksByUserIdAsync_Should_Trigger_Catch_And_Return_Empty_List()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var brokenContext = new LitShareDbContext(options);
            await brokenContext.Database.EnsureDeletedAsync(); // зламаємо БД

            var service = new BookService(brokenContext);

            // Act
            var result = await service.GetBooksByUserIdAsync(123);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result); // ✅ catch повинен спрацювати
        }
    }
}