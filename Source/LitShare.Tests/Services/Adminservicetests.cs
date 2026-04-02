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
        private readonly Mock<ILogger<AdminService>> loggerMock;
        private readonly AdminService sut;

        public AdminServiceTests()
        {
            this.complaintRepositoryMock = new Mock<IComplaintRepository>();
            this.loggerMock = new Mock<ILogger<AdminService>>();

            this.sut = new AdminService(
                this.complaintRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WithComplaints_ReturnsSuccess()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SampleComplaints());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WithComplaints_ReturnsCorrectCount()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SampleComplaints());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Equal(2, result.Value!.Count);
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
        public async Task GetAllComplaintsAsync_MapsTextCorrectly()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SampleComplaints());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Contains(result.Value!, c => c.Text == "Книга не відповідає опису");
        }

        [Fact]
        public async Task GetAllComplaintsAsync_MapsBookTitleFromPost()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SampleComplaints());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Contains(result.Value!, c => c.BookTitle == "Кобзар");
        }

        [Fact]
        public async Task GetAllComplaintsAsync_MapsComplainantNameFromUser()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(SampleComplaints());

            var result = await this.sut.GetAllComplaintsAsync();

            Assert.Contains(result.Value!, c => c.ComplainantName == "Іван");
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
    }
}