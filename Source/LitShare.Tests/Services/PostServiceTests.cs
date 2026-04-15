namespace LitShare.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LitShare.BLL.Services;
    using LitShare.DAL.Models;
    using LitShare.DAL.Repositories.Interfaces;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> postRepositoryMock;
        private readonly Mock<ILogger<PostService>> loggerMock;
        private readonly PostService sut;

        public PostServiceTests()
        {
            this.postRepositoryMock = new Mock<IPostRepository>();
            this.loggerMock = new Mock<ILogger<PostService>>();

            this.sut = new PostService(
                this.postRepositoryMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_ReturnsSuccess()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_ReturnsCorrectId()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal(1, result.Value!.Id);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsTitleCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal("Кобзар", result.Value!.Title);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsAuthorCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal("Тарас Шевченко", result.Value!.Author);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsDescriptionCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal("Збірка поезій", result.Value!.Description);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsPhotoUrlCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal("/images/posts/kobzar.jpg", result.Value!.PhotoUrl);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsDealTypeCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal(DealType.Exchange, result.Value!.DealType);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsUserIdCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal(42, result.Value!.UserId);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_MapsGenresCorrectly()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Equal(2, result.Value!.Genres.Count);
            Assert.Contains("Поезія", result.Value!.Genres);
            Assert.Contains("Класика", result.Value!.Genres);
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostWithNoPhoto_PhotoUrlIsNull()
        {
            var post = SamplePost();
            post.PhotoUrl = null;
            this.SetupGetByIdWithGenres(post);
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Null(result.Value!.PhotoUrl);
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostWithNoDescription_DescriptionIsNull()
        {
            var post = SamplePost();
            post.Description = null;
            this.SetupGetByIdWithGenres(post);
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Null(result.Value!.Description);
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostWithNoGenres_ReturnsEmptyGenresList()
        {
            var post = SamplePost();
            post.BookGenres = new List<BookGenres>();
            this.SetupGetByIdWithGenres(post);
            var result = await this.sut.GetPostDetailsAsync(1);
            Assert.Empty(result.Value!.Genres);
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_LogsStartMessage()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            await this.sut.GetPostDetailsAsync(1);
            this.VerifyLog(LogLevel.Information, "Fetching post details for ID: 1");
        }

        [Fact]
        public async Task GetPostDetailsAsync_ExistingPost_LogsSuccessMessage()
        {
            this.SetupGetByIdWithGenres(SamplePost());
            await this.sut.GetPostDetailsAsync(1);
            this.VerifyLog(LogLevel.Information, "Successfully fetched post details for ID: 1");
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostNotFound_ReturnsFailure()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdWithGenresAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);
            var result = await this.sut.GetPostDetailsAsync(99);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostNotFound_ReturnsCorrectErrorMessage()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdWithGenresAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);
            var result = await this.sut.GetPostDetailsAsync(99);
            Assert.Equal("Post with ID 99 not found", result.Error);
        }

        [Fact]
        public async Task GetPostDetailsAsync_PostNotFound_LogsWarning()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdWithGenresAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);
            await this.sut.GetPostDetailsAsync(99);
            this.VerifyLog(LogLevel.Warning, "Post not found. ID: 99");
        }

        [Fact]
        public async Task GetPostDetailsAsync_RepositoryThrows_PropagatesException()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdWithGenresAsync(It.IsAny<int>()))
                .ThrowsAsync(new InvalidOperationException("DB connection lost"));
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => this.sut.GetPostDetailsAsync(1));
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_WithPosts_ReturnsSuccess()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleUserPosts());
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_WithPosts_ReturnsCorrectCount()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleUserPosts());
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.Equal(2, result.Value!.Count());
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_WithPosts_MapsTitleCorrectly()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleUserPosts());
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.Contains(result.Value!, p => p.Title == "Кобзар");
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_WithNoPosts_ReturnsEmptyList()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(new List<Posts>());
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value!);
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_WithPosts_MapsCityFromUser()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(SampleUserPosts());
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.Contains(result.Value!, p => p.City == "Львів");
        }

        [Fact]
        public async Task GetPostsByUserIdAsync_UserWithNoCity_ReturnsFallbackCity()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByUserIdAsync(1))
                .ReturnsAsync(new List<Posts>
                {
                    new Posts { Id = 1, Title = "Книга", Author = "Автор", User = null },
                });
            var result = await this.sut.GetPostsByUserIdAsync(1, null);
            Assert.Contains(result.Value!, p => p.City == "Не вказано");
        }

        private void SetupGetByIdWithGenres(Posts post)
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdWithGenresAsync(post.Id))
                .ReturnsAsync(post);
        }

        private void VerifyLog(LogLevel level, string messagePart)
        {
            this.loggerMock.Verify(
                l => l.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(messagePart)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        private static Posts SamplePost() => new Posts
        {
            Id = 1,
            UserId = 42,
            Title = "Кобзар",
            Author = "Тарас Шевченко",
            Description = "Збірка поезій",
            PhotoUrl = "/images/posts/kobzar.jpg",
            DealType = DealType.Exchange,
            BookGenres = new List<BookGenres>
            {
                new BookGenres { Genre = new Genres { Name = "Поезія" } },
                new BookGenres { Genre = new Genres { Name = "Класика" } },
            },
        };

        private static IEnumerable<Posts> SampleUserPosts() => new List<Posts>
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
        [Fact]
        public async Task SetPostStatusAsync_ValidRequest_UpdatesStatusSuccessfully()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            var result = await this.sut.SetPostStatusAsync(1, 42, false);

            Assert.True(result.IsSuccess);
            Assert.False(post.IsActive);
        }

        [Fact]
        public async Task SetPostStatusAsync_PostNotFound_ReturnsFailure()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            var result = await this.sut.SetPostStatusAsync(1, 42, false);

            Assert.False(result.IsSuccess);
            Assert.Equal("Пост не знайдено", result.Error);
        }

        [Fact]
        public async Task SetPostStatusAsync_UnauthorizedUser_ReturnsFailure()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 99,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            var result = await this.sut.SetPostStatusAsync(1, 42, false);

            Assert.False(result.IsSuccess);
            Assert.Equal("Немає доступу до цього поста", result.Error);
        }

        [Fact]
        public async Task SetPostStatusAsync_CallsUpdateAsync()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.SetPostStatusAsync(1, 42, false);

            this.postRepositoryMock.Verify(r => r.UpdateAsync(post), Times.Once);
        }

        [Fact]
        public async Task SetPostStatusAsync_LogsStartMessage()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.SetPostStatusAsync(1, 42, false);

            this.VerifyLog(LogLevel.Information, "Changing post status");
        }

        [Fact]
        public async Task SetPostStatusAsync_LogsWarning_WhenPostNotFound()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            await this.sut.SetPostStatusAsync(1, 42, false);

            this.VerifyLog(LogLevel.Warning, "Post not found");
        }

        [Fact]
        public async Task SetPostStatusAsync_LogsWarning_WhenUnauthorized()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 99,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.SetPostStatusAsync(1, 42, false);

            this.VerifyLog(LogLevel.Warning, "Unauthorized");
        }

        [Fact]
        public async Task SetPostStatusAsync_LogsSuccessMessage()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42,
                IsActive = true
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.SetPostStatusAsync(1, 42, false);

            this.VerifyLog(LogLevel.Information, "Post status updated successfully");
        }

        [Fact]
        public async Task SetPostStatusAsync_ToggleFromFalseToTrue()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42,
                IsActive = false
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            var result = await this.sut.SetPostStatusAsync(1, 42, true);

            Assert.True(result.IsSuccess);
            Assert.True(post.IsActive);
        }
        [Fact]
        public async Task DeletePostAsync_ValidPost_DeletesSuccessfully()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            var result = await this.sut.DeletePostAsync(1, 42);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task DeletePostAsync_CallsDeleteAndSaveChanges()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.DeletePostAsync(1, 42);

            this.postRepositoryMock.Verify(r => r.DeleteAsync(post), Times.Once);
            this.postRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeletePostAsync_PostNotFound_ReturnsFailure()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            var result = await this.sut.DeletePostAsync(1, 42);

            Assert.False(result.IsSuccess);
            Assert.Equal("Пост не знайдено", result.Error);
        }

        [Fact]
        public async Task DeletePostAsync_UnauthorizedUser_ReturnsFailure()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 99
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            var result = await this.sut.DeletePostAsync(1, 42);

            Assert.False(result.IsSuccess);
            Assert.Equal("Немає доступу до цього поста", result.Error);
        }

        [Fact]
        public async Task DeletePostAsync_LogsStartMessage()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.DeletePostAsync(1, 42);

            this.VerifyLog(LogLevel.Information, "Deleting post");
        }

        [Fact]
        public async Task DeletePostAsync_LogsWarning_WhenPostNotFound()
        {
            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Posts?)null);

            await this.sut.DeletePostAsync(1, 42);

            this.VerifyLog(LogLevel.Warning, "Post not found");
        }

        [Fact]
        public async Task DeletePostAsync_LogsWarning_WhenUnauthorized()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 99
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.DeletePostAsync(1, 42);

            this.VerifyLog(LogLevel.Warning, "Unauthorized");
        }

        [Fact]
        public async Task DeletePostAsync_LogsSuccessMessage()
        {
            var post = new Posts
            {
                Id = 1,
                UserId = 42
            };

            this.postRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(post);

            await this.sut.DeletePostAsync(1, 42);

            this.VerifyLog(LogLevel.Information, "Post deleted successfully");
        }
    }
}