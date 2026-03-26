namespace LitShare.Tests.Services
{
    using LitShare.BLL.DTOs;
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
        public async Task GetFilteredPostsAsync_WithPosts_ReturnsSuccess()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithPosts_ReturnsCorrectCount()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Equal(2, result.Value!.Count);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithPosts_MapsTitleCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Contains(result.Value!, p => p.Title == "Кобзар");
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithPosts_MapsAuthorCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Contains(result.Value!, p => p.Author == "Тарас Шевченко");
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithPosts_MapsCityFromUser()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Contains(result.Value!, p => p.City == "Львів");
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithPosts_MapsDealTypeCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Contains(result.Value!, p => p.DealType == DealType.Exchange);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithNoPosts_ReturnsSuccessWithEmptyList()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(new List<Posts>());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_PostWithoutPhoto_PhotoUrlIsNull()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Книга", Author = "Автор", PhotoUrl = null, User = new Users { City = "Київ" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Null(result.Value!.First().PhotoUrl);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_PostWithoutUser_CityIsNull()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Книга", Author = "Автор", User = null },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto());

            Assert.Null(result.Value!.First().City);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WhenRepositoryThrows_PropagatesException()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetFilteredPostsAsync(new PostFilterDto()));
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithSearchTerm_PassesSearchTermToRepository()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync("Шевченко", null, null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                SearchTerm = "Шевченко",
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal("Кобзар", result.Value![0].Title);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithSearchTerm_MapsAuthorCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync("Шевченко", null, null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                SearchTerm = "Шевченко",
            });

            Assert.Equal("Тарас Шевченко", result.Value![0].Author);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_SearchTermNoMatch_ReturnsEmptyList()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync("НеіснуючийАвтор", null, null, null))
                .ReturnsAsync(new List<Posts>());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                SearchTerm = "НеіснуючийАвтор",
            });

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithLocation_PassesLocationToRepository()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, "Львів", null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                Location = "Львів",
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal("Львів", result.Value![0].City);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_LocationNoMatch_ReturnsEmptyList()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, "Тернопіль", null, null))
                .ReturnsAsync(new List<Posts>());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                Location = "Тернопіль",
            });

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }


        [Fact]
        public async Task GetFilteredPostsAsync_WithDealTypeExchange_PassesExchangeStringToRepository()
        {
            var expectedDealTypes = new List<string> { "exchange" };

            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, expectedDealTypes))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                DealType = DealType.Exchange,
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal(DealType.Exchange, result.Value![0].DealType);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithDealTypeDonation_PassesDonationStringToRepository()
        {
            var expectedDealTypes = new List<string> { "donation" };

            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, expectedDealTypes))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 2, Title = "Лісова пісня", Author = "Леся Українка", DealType = DealType.Donation, User = new Users { City = "Київ" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                DealType = DealType.Donation,
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal(DealType.Donation, result.Value![0].DealType);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_WithNoDealType_PassesNullToRepository()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, null, null))
                .ReturnsAsync(SamplePosts());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                DealType = null,
            });

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Count);

            this.postRepositoryMock.Verify(
                r => r.GetFilteredAsync(null, null, null, null),
                Times.Once);
        }


        [Fact]
        public async Task GetFilteredPostsAsync_WithGenres_PassesGenreIdsToRepository()
        {
            var genreIds = new List<int> { 1, 2 };

            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, genreIds, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                GenreIds = genreIds,
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_GenresNoMatch_ReturnsEmptyList()
        {
            var genreIds = new List<int> { 99 };

            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync(null, null, genreIds, null))
                .ReturnsAsync(new List<Posts>());

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                GenreIds = genreIds,
            });

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }


        [Fact]
        public async Task GetFilteredPostsAsync_SearchTermAndLocation_PassesBothToRepository()
        {
            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync("Шевченко", "Львів", null, null))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                SearchTerm = "Шевченко",
                Location = "Львів",
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.Equal("Львів", result.Value![0].City);
        }

        [Fact]
        public async Task GetFilteredPostsAsync_AllFiltersSet_PassesAllToRepository()
        {
            var genreIds = new List<int> { 1 };
            var dealTypes = new List<string> { "exchange" };

            this.postRepositoryMock
                .Setup(r => r.GetFilteredAsync("Шевченко", "Львів", genreIds, dealTypes))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Кобзар", Author = "Тарас Шевченко", DealType = DealType.Exchange, User = new Users { City = "Львів" } },
                });

            var result = await this.sut.GetFilteredPostsAsync(new PostFilterDto
            {
                SearchTerm = "Шевченко",
                Location = "Львів",
                GenreIds = genreIds,
                DealType = DealType.Exchange,
            });

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
        }

        private static IEnumerable<Posts> SamplePosts() => new List<Posts>
        {
            new Posts
            {
                Id = 1,
                Title = "Кобзар",
                Author = "Тарас Шевченко",
                DealType = DealType.Exchange,
                PhotoUrl = "/images/posts/kobzar.jpg",
                User = new Users { City = "Львів" },
            },
            new Posts
            {
                Id = 2,
                Title = "Тіні забутих предків",
                Author = "Михайло Коцюбинський",
                DealType = DealType.Donation,
                PhotoUrl = null,
                User = new Users { City = "Київ" },
            },
        };
    }
}