namespace LitShare.Tests.Services
{
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> favoriteRepositoryMock;
        private readonly Mock<ILogger<FavoriteService>> loggerMock;
        private readonly FavoriteService sut;

        public FavoriteServiceTests()
        {
            this.favoriteRepositoryMock = new Mock<IFavoriteRepository>();
            this.loggerMock = new Mock<ILogger<FavoriteService>>();

            this.sut = new FavoriteService(
                this.favoriteRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetFavoritesAsync_WithFavorites_ReturnsSuccess()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleFavorites());

            var result = await this.sut.GetFavoritesAsync(1);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetFavoritesAsync_WithFavorites_ReturnsCorrectCount()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleFavorites());

            var result = await this.sut.GetFavoritesAsync(1);

            Assert.Equal(2, result.Value!.Count);
        }

        [Fact]
        public async Task GetFavoritesAsync_WithNoFavorites_ReturnsEmptyList()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(new List<Favorites>());

            var result = await this.sut.GetFavoritesAsync(1);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task AddToFavoritesAsync_WhenNotFavorited_ReturnsTrue()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(1, 10))
                .ReturnsAsync(false);

            var result = await this.sut.AddToFavoritesAsync(1, 10);

            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
        }

        [Fact]
        public async Task AddToFavoritesAsync_WhenNotFavorited_CallsAddAsync()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(1, 10))
                .ReturnsAsync(false);

            await this.sut.AddToFavoritesAsync(1, 10);

            this.favoriteRepositoryMock.Verify(
                r => r.AddAsync(It.Is<Favorites>(f => f.UserId == 1 && f.PostId == 10)),
                Times.Once);
        }

        [Fact]
        public async Task AddToFavoritesAsync_WhenAlreadyFavorited_ReturnsFailure()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(1, 10))
                .ReturnsAsync(true);

            var result = await this.sut.AddToFavoritesAsync(1, 10);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task AddToFavoritesAsync_WhenAlreadyFavorited_NeverCallsAddAsync()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.ExistsAsync(1, 10))
                .ReturnsAsync(true);

            await this.sut.AddToFavoritesAsync(1, 10);

            this.favoriteRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<Favorites>()),
                Times.Never);
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_WhenFavorited_ReturnsTrue()
        {
            var favorite = new Favorites { UserId = 1, PostId = 10 };
            this.favoriteRepositoryMock
                .Setup(r => r.GetAsync(1, 10))
                .ReturnsAsync(favorite);

            var result = await this.sut.RemoveFromFavoritesAsync(1, 10);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_WhenFavorited_CallsRemoveAsync()
        {
            var favorite = new Favorites { UserId = 1, PostId = 10 };
            this.favoriteRepositoryMock
                .Setup(r => r.GetAsync(1, 10))
                .ReturnsAsync(favorite);

            await this.sut.RemoveFromFavoritesAsync(1, 10);

            this.favoriteRepositoryMock.Verify(
                r => r.RemoveAsync(favorite),
                Times.Once);
        }

        [Fact]
        public async Task RemoveFromFavoritesAsync_WhenNotFavorited_ReturnsFailure()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.GetAsync(1, 10))
                .ReturnsAsync((Favorites?)null);

            var result = await this.sut.RemoveFromFavoritesAsync(1, 10);

            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task GetFavoritePostIdsAsync_ReturnsCorrectIds()
        {
            var ids = new HashSet<int> { 1, 2, 3 };
            this.favoriteRepositoryMock
                .Setup(r => r.GetFavoritePostIdsAsync(1))
                .ReturnsAsync(ids);

            var result = await this.sut.GetFavoritePostIdsAsync(1);

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value!.Count);
        }

        [Fact]
        public async Task GetFavoritePostIdsAsync_WhenRepositoryThrows_PropagatesException()
        {
            this.favoriteRepositoryMock
                .Setup(r => r.GetFavoritePostIdsAsync(1))
                .ThrowsAsync(new InvalidOperationException("DB error"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetFavoritePostIdsAsync(1));
        }

        private static IEnumerable<Favorites> SampleFavorites() => new List<Favorites>
        {
            new Favorites
            {
                UserId = 1,
                PostId = 10,
                Post = new Posts { Id = 10, Title = "Кобзар", Author = "Шевченко", User = new Users { City = "Львів" } },
            },
            new Favorites
            {
                UserId = 1,
                PostId = 20,
                Post = new Posts { Id = 20, Title = "Тіні", Author = "Коцюбинський", User = new Users { City = "Київ" } },
            },
        };
    }
}