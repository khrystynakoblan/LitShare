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
            Assert.Equal("Очікує", result[0].Status);
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

                [Fact]
        public async Task GetReceivedRequestsAsync_ReturnsSuccess_WhenRequestsExist()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Id = 1,
                    SenderId = 10,
                    PostId = 5,
                    Status = RequestStatus.Pending,
                    Sender = new Users { Name = "User1" },
                    Post = new Posts { Title = "Book1" }
                }
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value!);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_ShouldReturnSuccessWithEmptyList_WhenNoRequestsFound()
        {
            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<ExchangeRequest>());

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_MapsStatus_Pending()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Status = RequestStatus.Pending,
                    Sender = new Users(),
                    Post = new Posts()
                }
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.Equal("Очікує", result.Value![0].Status);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_MapsStatus_Accepted()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Status = RequestStatus.Accepted,
                    Sender = new Users(),
                    Post = new Posts()
                }
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.Equal("Прийнято", result.Value![0].Status);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_MapsStatus_Rejected()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Status = RequestStatus.Rejected,
                    Sender = new Users(),
                    Post = new Posts()
                }
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.Equal("Відхилено", result.Value![0].Status);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_UsesFallback_WhenNull()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest
                {
                    Status = RequestStatus.Pending,
                    Sender = null!,
                    Post = null!
                }
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            var dto = result.Value![0];

            Assert.Equal("Невідомий користувач", dto.SenderName);
            Assert.Equal("Без назви", dto.PostTitle);
        }

        [Fact]
        public async Task GetReceivedRequestsAsync_ReturnsCorrectCount()
        {
            var requests = new List<ExchangeRequest>
            {
                new ExchangeRequest(),
                new ExchangeRequest(),
                new ExchangeRequest()
            };

            _exchangeRepoMock
                .Setup(r => r.GetReceivedRequestsAsync(1))
                .ReturnsAsync(requests);

            var result = await _service.GetReceivedRequestsAsync(1);

            Assert.Equal(3, result.Value!.Count);
        }

        [Fact]
        public async Task UpdateRequestStatusAsync_ShouldReturnFailure_WhenRequestNotFound()
        {
            _exchangeRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                             .ReturnsAsync((ExchangeRequest)null!);

            var result = await _service.UpdateRequestStatusAsync(1, 10, RequestStatus.Accepted);

            Assert.False(result.IsSuccess);
            Assert.Equal("Запит не знайдено.", result.Error);
        }

        [Fact]
        public async Task UpdateRequestStatusAsync_ShouldReturnFailure_WhenUserIsNotOwner()
        {
            var request = new ExchangeRequest
            {
                Post = new Posts { UserId = 99 }
            };
            _exchangeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

            var result = await _service.UpdateRequestStatusAsync(1, 10, RequestStatus.Accepted);

            Assert.False(result.IsSuccess);
            Assert.Equal("Ви не маєте прав для цієї дії.", result.Error);
        }

        [Fact]
        public async Task UpdateRequestStatusAsync_ShouldReturnFailure_WhenStatusIsNotPending()
        {
            var request = new ExchangeRequest
            {
                Post = new Posts { UserId = 10 },
                Status = RequestStatus.Accepted
            };
            _exchangeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

            var result = await _service.UpdateRequestStatusAsync(1, 10, RequestStatus.Rejected);

            Assert.False(result.IsSuccess);
            Assert.Equal("Цей запит вже був опрацьований.", result.Error);
        }

        [Fact]
        public async Task UpdateRequestStatusAsync_ShouldReturnSuccess_AndChangeStatus()
        {
            var request = new ExchangeRequest
            {
                Id = 1,
                Post = new Posts { UserId = 10 },
                Status = RequestStatus.Pending
            };
            _exchangeRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(request);

            var result = await _service.UpdateRequestStatusAsync(1, 10, RequestStatus.Accepted);

            Assert.True(result.IsSuccess);
            Assert.Equal(RequestStatus.Accepted, request.Status);
            _exchangeRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}