namespace LitShare.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LitShare.BLL.Common;
    using LitShare.BLL.DTOs;
    using LitShare.BLL.Services.Interfaces;
    using LitShare.Web.Controllers;
    using LitShare.Web.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class GuestProfileTests
    {
        private readonly Mock<IProfileService> profileServiceMock;
        private readonly Mock<IPostService> postServiceMock;
        private readonly Mock<IReviewService> reviewServiceMock;
        private readonly Mock<IFavoriteService> favoriteServiceMock;
        private readonly Mock<ILogger<ProfileController>> loggerMock;
        private readonly ProfileController controller;

        public GuestProfileTests()
        {
            this.profileServiceMock = new Mock<IProfileService>();
            this.postServiceMock = new Mock<IPostService>();
            this.reviewServiceMock = new Mock<IReviewService>();
            this.favoriteServiceMock = new Mock<IFavoriteService>();
            this.loggerMock = new Mock<ILogger<ProfileController>>();

            this.reviewServiceMock
                .Setup(r => r.GetReviewsByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(Result<IEnumerable<ReviewDto>>.Success(new List<ReviewDto>()));

            this.controller = new ProfileController(
                this.profileServiceMock.Object,
                this.postServiceMock.Object,
                this.reviewServiceMock.Object,
                this.favoriteServiceMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task GuestProfile_FullSuccess_MapsAllFieldsCorrectly()
        {
            int userId = 1;
            var user = new UserProfileDto { Id = userId, Name = "User1", City = "City1", Phone = "+380970000000" };
            var books = new List<PostCardDto> { new PostCardDto { Title = "Book1" } };

            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(Result<UserProfileDto>.Success(user));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(userId, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(books));

            var result = await this.controller.GuestProfile(userId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("User1", model.Name);
            Assert.Equal("+380970000000", model.Phone);
            Assert.Single(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_UserNotFound_ReturnsNotFound()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(Result<UserProfileDto>.Failure("Error"));

            var result = await this.controller.GuestProfile(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GuestProfile_PostServiceFails_ReturnsViewWithEmptyBooks()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { Name = "User1" }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Failure("DB Error"));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Empty(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_NameIsNull_SetsDefaultName()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { Name = null! }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Користувач", model.Name);
        }

        [Fact]
        public async Task GuestProfile_LocationsAreNull_SetsDefaultText()
        {
            var user = new UserProfileDto { Name = "User1", Region = null, City = null };
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(user));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Не вказано", model.Region);
            Assert.Equal("Не вказано", model.City);
        }

        [Fact]
        public async Task GuestProfile_AboutIsNull_SetsDefaultText()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { About = null }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Інформація відсутня", model.About);
        }

        [Fact]
        public async Task GuestProfile_PhotoUrl_PassesCorrectUrl()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { PhotoUrl = "test.jpg" }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("test.jpg", model.PhotoUrl);
        }

        [Fact]
        public async Task GuestProfile_NoBooks_ReturnsEmptyCollection()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto()));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Empty(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_ManyBooks_PassesAllBooksToModel()
        {
            var books = new List<PostCardDto> { new PostCardDto(), new PostCardDto() };
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto()));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(books));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(2, model.UserBooks.Count());
        }

        [Fact]
        public async Task GuestProfile_EmailIsNull_SetsEmptyString()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { Email = null! }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(string.Empty, model.Email);
        }

        [Fact]
        public async Task GuestProfile_ReviewServiceFails_SetsReviewCountToZero()
        {
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { Name = "User1" }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1,true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));
            this.reviewServiceMock.Setup(r => r.GetReviewsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<ReviewDto>>.Failure("Error"));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(0, model.ReviewCount);
        }

        [Fact]
        public async Task GuestProfile_WithReviews_SetsCorrectReviewCount()
        {
            var reviews = new List<ReviewDto> { new ReviewDto(), new ReviewDto(), new ReviewDto() };
            this.profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<UserProfileDto>.Success(new UserProfileDto { Name = "User1" }));
            this.postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1, true)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));
            this.reviewServiceMock.Setup(r => r.GetReviewsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<ReviewDto>>.Success(reviews));

            var result = await this.controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(3, model.ReviewCount);
        }
    }
}