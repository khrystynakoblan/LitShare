namespace LitShare.Tests.Services
{
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class HomeServiceTests
    {
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<ILogger<HomeService>> loggerMock;
        private readonly HomeService sut;

        public HomeServiceTests()
        {
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.loggerMock = new Mock<ILogger<HomeService>>();

            this.sut = new HomeService(
                this.postRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPosts_ReturnsSuccess()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetAllPostsAsync();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPosts_ReturnsCorrectCount()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetAllPostsAsync();

            Assert.Equal(2, result.Value!.Count());
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPosts_MapsTitleCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetAllPostsAsync();

            Assert.Contains(result.Value!, p => p.Title == "Кобзар");
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPosts_MapsAuthorCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetAllPostsAsync();

            Assert.Contains(result.Value!, p => p.Author == "Тарас Шевченко");
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPosts_MapsCityFromUser()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetAllPostsAsync();

            Assert.Contains(result.Value!, p => p.City == "Львів");
        }

        [Fact]
        public async Task GetAllPostsAsync_WithNoPosts_ReturnsSuccessWithEmptyList()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Posts>());

            var result = await this.sut.GetAllPostsAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetAllPostsAsync_WithPostWithoutPhoto_PhotoUrlIsNull()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Книга", Author = "Автор", PhotoUrl = null },
                });

            var result = await this.sut.GetAllPostsAsync();

            Assert.Null(result.Value!.First().PhotoUrl);
        }

        [Fact]
        public async Task GetAllPostsAsync_WhenRepositoryThrows_PropagatesException()
        {
            this.postRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetAllPostsAsync());
        }

        private static IEnumerable<Posts> SamplePosts() => new List<Posts>
        {
            new Posts
            {
                Id = 1,
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                PhotoUrl = "/images/posts/kobzar.jpg",
                User = new Users { City = "Львів" },
            },
            new Posts
            {
                Id = 2,
                Title = "Тіні забутих предків",
                Author = "Михайло Коцюбинський",
                PhotoUrl = null,
                User = new Users { City = "Київ" },
            },
        };
    }
}