namespace LitShare.Tests.Services
{
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class AdminServiceTests
    {
        private readonly Mock<IComplaintRepository> complaintRepositoryMock;
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<ILogger<AdminService>> loggerMock;
        private readonly AdminService sut;

        public AdminServiceTests()
        {
            this.complaintRepositoryMock = new Mock<IComplaintRepository>();
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.loggerMock = new Mock<ILogger<AdminService>>();

            this.sut = new AdminService(
                this.complaintRepositoryMock.Object,
                this.postRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WithNoComplaints_ReturnsSuccessWithEmptyList()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_MapsDateCorrectly()
        {
            var date = new DateTime(2025, 9, 24);

            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>
                {
                    new Complaints
                    {
                        Id = 1,
                        Text = "Скарга",
                        Date = date,
                        Post = new Posts { Title = "Кобзар" },
                        Complainant = new Users { Name = "Іван" },
                    },
                });

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(date, result.Value![0].Date);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_ComplaintWithNullText_ReturnsEmptyString()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>
                {
                    new Complaints
                    {
                        Id = 1,
                        Text = null,
                        Post = new Posts { Title = "Кобзар" },
                        Complainant = new Users { Name = "Іван" },
                    },
                });

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(string.Empty, result.Value![0].Text);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_ComplaintWithNullPost_ReturnsEmptyBookTitle()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>
                {
                    new Complaints
                    {
                        Id = 1,
                        Text = "Скарга",
                        Post = null,
                        Complainant = new Users { Name = "Іван" },
                    },
                });

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(string.Empty, result.Value![0].BookTitle);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_ComplaintWithNullComplainant_ReturnsEmptyComplainantName()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>
                {
                    new Complaints
                    {
                        Id = 1,
                        Text = "Скарга",
                        Post = new Posts { Title = "Кобзар" },
                        Complainant = null,
                    },
                });

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(string.Empty, result.Value![0].ComplainantName);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_ComplaintWithNullPostTitle_ReturnsEmptyBookTitle()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>
                {
                    new Complaints
                    {
                        Id = 1,
                        Text = "Скарга",
                        Post = new Posts { Title = null },
                        Complainant = new Users { Name = "Іван" },
                    },
                });

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(string.Empty, result.Value![0].BookTitle);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WhenRepositoryThrows_PropagatesException()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetAllComplaintsAsync());
        }

        private static IEnumerable<Complaints> SampleComplaints() => new List<Complaints>
        {
            new Complaints
            {
                Id = 1,
                Text = "Книга не відповідає опису",
                Date = new DateTime(2025, 9, 24),
                Post = new Posts { Title = "Кобзар" },
                Complainant = new Users { Name = "Іван" },
            },
            new Complaints
            {
                Id = 2,
                Text = "Продавець не виходить на зв'язок",
                Date = new DateTime(2025, 9, 24),
                Post = new Posts { Title = "Лісова пісня" },
                Complainant = new Users { Name = "Марія" },
            },
        };

        [Fact]
        public async Task ApproveComplaintAsync_ComplaintNotFound_ReturnsFailure()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Complaints?)null);

            var result = await this.sut.ApproveComplaintAsync(1);

            Assert.True(result.IsFailure);
            Assert.Equal("Скаргу не знайдено.", result.Error);
        }

        [Fact]
        public async Task ApproveComplaintAsync_WithExistingPost_DeletesBothPostAndComplaint()
        {
            var post = new Posts { Id = 10, Title = "Книга з порушенням" };
            var complaint = new Complaints { Id = 1, Post = post };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            var result = await this.sut.ApproveComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(r => r.DeleteAsync(post), Times.Once);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
            this.complaintRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveComplaintAsync_PostIsNull_DeletesOnlyComplaint()
        {
            var complaint = new Complaints { Id = 1, Post = null };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            var result = await this.sut.ApproveComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Posts>()), Times.Never);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
        }

        [Fact]
        public async Task RejectComplaintAsync_ComplaintNotFound_ReturnsFailure()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Complaints?)null);

            var result = await this.sut.RejectComplaintAsync(1);

            Assert.True(result.IsFailure);
            Assert.Equal("Скаргу не знайдено.", result.Error);
        }

        [Fact]
        public async Task RejectComplaintAsync_ComplaintExists_DeletesOnlyComplaintNotPost()
        {
            var post = new Posts { Id = 10, Title = "Хороша книга" };
            var complaint = new Complaints { Id = 1, Post = post };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            var result = await this.sut.RejectComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
            this.postRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Posts>()), Times.Never);
            this.complaintRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetComplaintByIdAsync_ExistingId_ReturnsMappedDto()
        {
            var complaint = new Complaints
            {
                Id = 1,
                Text = "Тест",
                Post = new Posts { Title = "Кобзар", Author = "Шевченко" },
                Complainant = new Users { Name = "Олег" }
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            var result = await this.sut.GetComplaintByIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.Equal("Кобзар", result.Value!.BookTitle);
            Assert.Equal("Олег", result.Value!.ComplainantName);
        }
    }
}