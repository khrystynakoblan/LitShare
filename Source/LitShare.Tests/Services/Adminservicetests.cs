namespace LitShare.Tests.Services
{
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class AdminServiceTests
    {
        private readonly Mock<IComplaintRepository> complaintRepositoryMock;
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<ILogger<AdminService>> loggerMock;
        private readonly IOptions<AppSettings> options;
        private readonly AdminService adminService;

        public AdminServiceTests()
        {
            this.complaintRepositoryMock = new Mock<IComplaintRepository>();
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.loggerMock = new Mock<ILogger<AdminService>>();

            var appSettings = new AppSettings
            {
                AdminStatsTopCitiesCount = 5,
                AdminStatsTopGenresCount = 5
            };
            this.options = Options.Create(appSettings);

            this.adminService = new AdminService(
                this.complaintRepositoryMock.Object,
                this.postRepositoryMock.Object,
                this.userRepositoryMock.Object,
                this.loggerMock.Object,
                this.options);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WhenComplaintsExist_ReturnsSuccessWithList()
        {
            var complaints = new List<Complaints>
            {
                new Complaints
                {
                    Id = 1,
                    Text = "Скарга 1",
                    Date = DateTime.Now,
                    Post = new Posts { Title = "Книга 1" },
                    Complainant = new Users { Name = "Користувач 1" }
                },
                new Complaints
                {
                    Id = 2,
                    Text = "Скарга 2",
                    Date = DateTime.Now,
                    Post = new Posts { Title = "Книга 2" },
                    Complainant = new Users { Name = "Користувач 2" }
                }
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(complaints);

            var result = await this.adminService.GetAllComplaintsAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value!.Count);
            Assert.Equal("Скарга 1", result.Value[0].Text);
            Assert.Equal("Книга 1", result.Value[0].BookTitle);
        }

        [Fact]
        public async Task GetAllComplaintsAsync_WhenNoComplaints_ReturnsSuccessWithEmptyList()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Complaints>());

            var result = await this.adminService.GetAllComplaintsAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetComplaintByIdAsync_WhenComplaintExists_ReturnsSuccessWithDetails()
        {
            var complaint = new Complaints
            {
                Id = 1,
                Text = "Тестова скарга",
                Date = new DateTime(2024, 1, 15),
                Post = new Posts
                {
                    Title = "Кобзар",
                    Author = "Тарас Шевченко",
                    Description = "Опис книги",
                    PhotoUrl = "/images/kobzar.jpg"
                },
                Complainant = new Users { Name = "Іван Петренко" }
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            var result = await this.adminService.GetComplaintByIdAsync(1);

            Assert.True(result.IsSuccess);
            Assert.Equal(1, result.Value!.Id);
            Assert.Equal("Тестова скарга", result.Value.Text);
            Assert.Equal("Іван Петренко", result.Value.ComplainantName);
            Assert.Equal("Кобзар", result.Value.BookTitle);
            Assert.Equal("Тарас Шевченко", result.Value.BookAuthor);
        }

        [Fact]
        public async Task GetComplaintByIdAsync_WhenComplaintNotFound_ReturnsFailure()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Complaints?)null);

            var result = await this.adminService.GetComplaintByIdAsync(999);

            Assert.True(result.IsFailure);
            Assert.Equal("Скаргу не знайдено.", result.Error);
        }

        
        [Fact]
        public async Task ApproveComplaintAsync_WhenComplaintExistsWithPost_DeletesPostAndComplaint()
        {
            var post = new Posts { Id = 10, Title = "Тестова книга" };
            var complaint = new Complaints
            {
                Id = 1,
                Text = "Скарга",
                Post = post
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            this.postRepositoryMock
                .Setup(r => r.DeletePostAsync(post))
                .Returns(Task.CompletedTask);

            this.postRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            this.complaintRepositoryMock
                .Setup(r => r.DeleteAsync(complaint))
                .Returns(Task.CompletedTask);

            var result = await this.adminService.ApproveComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(r => r.DeletePostAsync(post), Times.Once);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ApproveComplaintAsync_WhenComplaintExistsWithoutPost_DeletesOnlyComplaint()
        {
            var complaint = new Complaints
            {
                Id = 1,
                Text = "Скарга",
                Post = null
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            this.complaintRepositoryMock
                .Setup(r => r.DeleteAsync(complaint))
                .Returns(Task.CompletedTask);

            var result = await this.adminService.ApproveComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(r => r.DeletePostAsync(It.IsAny<Posts>()), Times.Never);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
        }

        [Fact]
        public async Task ApproveComplaintAsync_WhenComplaintNotFound_ReturnsFailure()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Complaints?)null);

            var result = await this.adminService.ApproveComplaintAsync(999);

            Assert.True(result.IsFailure);
            Assert.Equal("Скаргу не знайдено.", result.Error);
            this.postRepositoryMock.Verify(r => r.DeletePostAsync(It.IsAny<Posts>()), Times.Never);
        }

        
        [Fact]
        public async Task RejectComplaintAsync_WhenComplaintExists_DeletesOnlyComplaint()
        {
            var complaint = new Complaints
            {
                Id = 1,
                Text = "Скарга",
                Post = new Posts { Id = 10, Title = "Книга" }
            };

            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(complaint);

            this.complaintRepositoryMock
                .Setup(r => r.DeleteAsync(complaint))
                .Returns(Task.CompletedTask);

            var result = await this.adminService.RejectComplaintAsync(1);

            Assert.True(result.IsSuccess);
            this.postRepositoryMock.Verify(r => r.DeletePostAsync(It.IsAny<Posts>()), Times.Never);
            this.complaintRepositoryMock.Verify(r => r.DeleteAsync(complaint), Times.Once);
        }

        [Fact]
        public async Task RejectComplaintAsync_WhenComplaintNotFound_ReturnsFailure()
        {
            this.complaintRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Complaints?)null);

            var result = await this.adminService.RejectComplaintAsync(999);

            Assert.True(result.IsFailure);
            Assert.Equal("Скаргу не знайдено.", result.Error);
        }

        [Fact]
        public async Task GetStatisticsAsync_WhenDataExists_ReturnsCorrectStatistics()
        {
            var users = new List<Users>
            {
                new Users { Id = 1, Name = "User1", City = "Київ" },
                new Users { Id = 2, Name = "User2", City = "Львів" },
                new Users { Id = 3, Name = "User3", City = "Київ" }
            };

            var posts = new List<Posts>
            {
                new Posts
                {
                    Id = 1,
                    Title = "Книга 1",
                    User = new Users { City = "Київ" },
                    BookGenres = new List<BookGenres>
                    {
                        new BookGenres { Genre = new Genres { Name = "Роман" } }
                    }
                },
                new Posts
                {
                    Id = 2,
                    Title = "Книга 2",
                    User = new Users { City = "Львів" },
                    BookGenres = new List<BookGenres>
                    {
                        new BookGenres { Genre = new Genres { Name = "Поезія" } }
                    }
                },
                new Posts
                {
                    Id = 3,
                    Title = "Книга 3",
                    User = new Users { City = "Київ" },
                    BookGenres = new List<BookGenres>
                    {
                        new BookGenres { Genre = new Genres { Name = "Роман" } },
                        new BookGenres { Genre = new Genres { Name = "Драма" } }
                    }
                }
            };

            var complaints = new List<Complaints>
            {
                new Complaints { Id = 1, Text = "Скарга 1" },
                new Complaints { Id = 2, Text = "Скарга 2" }
            };

            this.userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users.AsEnumerable());
            this.postRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(posts.AsEnumerable());
            this.complaintRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(complaints.AsEnumerable());

            var result = await this.adminService.GetStatisticsAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Value!.TotalUsers);
            Assert.Equal(3, result.Value.TotalPosts);
            Assert.Equal(2, result.Value.TotalComplaints);

            Assert.Contains(result.Value.TopCities, c => c.City == "Київ" && c.Count == 2);
            Assert.Contains(result.Value.TopCities, c => c.City == "Львів" && c.Count == 1);

            Assert.Contains(result.Value.TopGenres, g => g.GenreName == "Роман" && g.Count == 2);
        }

        [Fact]
        public async Task GetStatisticsAsync_WhenNoData_ReturnsEmptyStatistics()
        {
            this.userRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Users>());
            this.postRepositoryMock.Setup(r => r.GetAllPostsAsync()).ReturnsAsync(new List<Posts>());
            this.complaintRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Complaints>());

            var result = await this.adminService.GetStatisticsAsync();

            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Value!.TotalUsers);
            Assert.Equal(0, result.Value.TotalPosts);
            Assert.Equal(0, result.Value.TotalComplaints);
            Assert.Empty(result.Value.TopCities);
            Assert.Empty(result.Value.TopGenres);
        }

        [Fact]
        public async Task GetStatisticsAsync_WhenRepositoryThrowsException_ReturnsFailure()
        {
            this.userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            var result = await this.adminService.GetStatisticsAsync();

            Assert.True(result.IsFailure);
            Assert.Equal("Не вдалося завантажити статистику", result.Error);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenUsersExist_ReturnsSuccessWithMappedDtos()
        {
            var users = new List<Users>
            {
                new Users
                {
                    Id = 1,
                    Name = "User1",
                    Email = "user1@example.com",
                    Phone = "+380970000000",
                    City = "Lviv",
                    Region = "Lvivska",
                    Role = RoleType.Admin
                },
                new Users
                {
                    Id = 2,
                    Name = null,
                    Email = null,
                    Phone = null,
                    City = "Kyiv",
                    Region = "Kyivska",
                    Role = RoleType.User
                }
            };

            this.userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            var result = await this.adminService.GetAllUsersAsync();

            Assert.True(result.IsSuccess);
            var dtos = result.Value!.ToList();
            Assert.Equal(2, dtos.Count);

            Assert.Equal("User1", dtos[0].Name);
            Assert.Equal("Lviv, Lvivska", dtos[0].Location);
            Assert.Equal("Admin", dtos[0].Role);

            Assert.Equal("Без імені", dtos[1].Name);
            Assert.Equal("Не вказано", dtos[1].Email);
            Assert.Equal("-", dtos[1].Phone);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenNoUsers_ReturnsSuccessWithEmptyList()
        {
            this.userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Users>());

            var result = await this.adminService.GetAllUsersAsync();

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenRepositoryThrowsException_ReturnsFailure()
        {
            this.userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database connection failed"));

            var result = await this.adminService.GetAllUsersAsync();

            Assert.True(result.IsFailure);
            Assert.Equal("Не вдалося завантажити список користувачів.", result.Error);
        }

        [Fact]
        public async Task ToggleUserBlockAsync_WhenUserExists_TogglesStatusAndCallsUpdate()
        {
            var user = new Users
            {
                Id = 1,
                Name = "Test User",
                Role = RoleType.User,
                IsBlocked = false
            };

            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            this.userRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Users>()))
                .Returns(Task.CompletedTask);

            var result = await this.adminService.ToggleUserBlockAsync(1);

            Assert.True(result.IsSuccess);
            Assert.True(user.IsBlocked);
            this.userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task ToggleUserBlockAsync_WhenUserIsAdmin_ReturnsFailure()
        {
            var adminUser = new Users
            {
                Id = 1,
                Name = "Big Boss",
                Role = RoleType.Admin,
                IsBlocked = false
            };

            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(adminUser);

            var result = await this.adminService.ToggleUserBlockAsync(1);

            Assert.True(result.IsFailure);
            Assert.Equal("Неможливо заблокувати адміністратора.", result.Error);
            Assert.False(adminUser.IsBlocked);
            this.userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Users>()), Times.Never);
        }

        [Fact]
        public async Task ToggleUserBlockAsync_WhenUserNotFound_ReturnsFailure()
        {
            this.userRepositoryMock
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Users?)null);

            var result = await this.adminService.ToggleUserBlockAsync(999);

            Assert.True(result.IsFailure);
            Assert.Equal("Користувача не знайдено.", result.Error);
            this.userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Users>()), Times.Never);
        }
    }
}