using LitShare.BLL.Services;
using LitShare.BLL.Services.Interfaces;
using LitShare.DAL.Models;
using LitShare.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LitShare.Tests.BLL.Services
{
    public class ExchangeServiceTests
    {
        private readonly Mock<IExchangeRepository> _exchangeRepoMock;
        private readonly Mock<IPostRepository> _postRepoMock;
        private readonly Mock<ILogger<ExchangeService>> _loggerMock;
        private readonly ExchangeService _service;

        public ExchangeServiceTests()
        {
            _exchangeRepoMock = new Mock<IExchangeRepository>();
            _postRepoMock = new Mock<IPostRepository>();
            _loggerMock = new Mock<ILogger<ExchangeService>>();

            _service = new ExchangeService(_exchangeRepoMock.Object, _postRepoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateRequestAsync_ShouldReturnFailure_WhenPostNotFound()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                         .ReturnsAsync((Posts)null!);

            var result = await _service.CreateRequestAsync(postId: 1, senderId: 1);

            Assert.False(result.IsSuccess);
            Assert.Equal("Оголошення не знайдено.", result.Error);
        }

        [Fact]
        public async Task CreateRequestAsync_ShouldReturnFailure_WhenSenderIsOwner()
        {
            var post = new Posts { Id = 1, UserId = 10 };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);

            var result = await _service.CreateRequestAsync(1, 10);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви не можете надіслати запит на власне оголошення.", result.Error);
        }

        [Fact]
        public async Task CreateRequestAsync_ShouldReturnFailure_WhenAlreadyExists()
        {
            var post = new Posts { Id = 1, UserId = 10 };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _exchangeRepoMock.Setup(r => r.ExistsAsync(2, 1)).ReturnsAsync(true);

            var result = await _service.CreateRequestAsync(1, 2);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви вже надсилали запит на це оголошення.", result.Error);
        }

        [Fact]
        public async Task CreateRequestAsync_ShouldReturnSuccess_WhenDataIsValid()
        {
            var post = new Posts { Id = 1, UserId = 10 };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _exchangeRepoMock.Setup(r => r.ExistsAsync(2, 1)).ReturnsAsync(false);

            var result = await _service.CreateRequestAsync(1, 2);

            Assert.True(result.IsSuccess);
            _exchangeRepoMock.Verify(r => r.AddAsync(It.IsAny<ExchangeRequest>()), Times.Once);
            _exchangeRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetSentRequestsAsync_ShouldReturnMappedDtos()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Id = 1,
                    PostId = 100,
                    Status = RequestStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    Post = new Posts { Title = "Кобзар", Author = "Т. Шевченко" }
                }
            };
            _exchangeRepoMock.Setup(r => r.GetBySenderIdAsync(1)).ReturnsAsync(requests);

            var result = (await _service.GetSentRequestsAsync(1)).ToList();

            Assert.Single(result);
            Assert.Equal("Кобзар", result[0].BookTitle);
            Assert.Equal(100, result[0].PostId);
            Assert.Equal("Pending", result[0].Status);
        }

        [Fact]
        public async Task GetSentRequestsAsync_ShouldReturnEmptyList_WhenUserHasNoRequests()
        {
            _exchangeRepoMock.Setup(r => r.GetBySenderIdAsync(It.IsAny<int>()))
                             .ReturnsAsync(new List<ExchangeRequest>());

            var result = await _service.GetSentRequestsAsync(1);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSentRequestsAsync_ShouldUseDefaultValues_WhenPostTitleOrAuthorIsNull()
        {
            var requests = new List<ExchangeRequest>
    {
        new ExchangeRequest
        {
            Id = 1,
            PostId = 100,
            Status = RequestStatus.Pending,
            Post = new Posts { Title = null!, Author = null! }
        }
    };
            _exchangeRepoMock.Setup(r => r.GetBySenderIdAsync(1)).ReturnsAsync(requests);

            var result = (await _service.GetSentRequestsAsync(1)).ToList();

            Assert.Equal("Без назви", result[0].BookTitle);
            Assert.Equal("Невідомий автор", result[0].BookAuthor);
        }
    }
}