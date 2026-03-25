using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using LitShare.Web.Controllers;
using LitShare.Web.Models;
using LitShare.BLL.Services.Interfaces;
using LitShare.BLL.DTOs;
using LitShare.BLL.Common;
using LitShare.DAL.Models;

namespace LitShare.Tests.Controllers
{
    public class GuestProfileTests
    {
        private readonly Mock<IProfileService> _profileServiceMock;
        private readonly Mock<IPostService> _postServiceMock;
        private readonly Mock<ILogger<ProfileController>> _loggerMock;
        private readonly ProfileController _controller;

        public GuestProfileTests()
        {
            _profileServiceMock = new Mock<IProfileService>();
            _postServiceMock = new Mock<IPostService>();
            _loggerMock = new Mock<ILogger<ProfileController>>();

            _controller = new ProfileController(
                _profileServiceMock.Object,
                _postServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task GuestProfile_FullSuccess_MapsAllFieldsCorrectly()
        {
            int userId = 1;
            var user = new Users { Id = userId, Name = "User1", City = "City1" };
            var books = new List<PostCardDto> { new PostCardDto { Title = "Book1" } };

            _profileServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(Result<Users>.Success(user));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(userId)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(books));

            var result = await _controller.GuestProfile(userId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("User1", model.Name);
            Assert.Single(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_UserNotFound_ReturnsNotFound()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(Result<Users>.Failure("Error"));

            var result = await _controller.GuestProfile(1);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GuestProfile_PostServiceFails_ReturnsViewWithEmptyBooks()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users { Name = "User1" }));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Failure("DB Error"));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Empty(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_NameIsNull_SetsDefaultName()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users { Name = null }));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Користувач", model.Name);
        }

        [Fact]
        public async Task GuestProfile_LocationsAreNull_SetsDefaultText()
        {
            var user = new Users { Name = "User1", Region = null, City = null };
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(user));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Не вказано", model.Region);
            Assert.Equal("Не вказано", model.City);
        }

        [Fact]
        public async Task GuestProfile_AboutIsNull_SetsDefaultText()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users { About = null }));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("Інформація відсутня", model.About);
        }

        [Fact]
        public async Task GuestProfile_PhotoUrl_PassesCorrectUrl()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users { PhotoUrl = "test.jpg" }));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal("test.jpg", model.PhotoUrl);
        }

        [Fact]
        public async Task GuestProfile_NoBooks_ReturnsEmptyCollection()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users()));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Empty(model.UserBooks);
        }

        [Fact]
        public async Task GuestProfile_ManyBooks_PassesAllBooksToModel()
        {
            var books = new List<PostCardDto> { new PostCardDto(), new PostCardDto() };
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users()));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(books));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(2, model.UserBooks.Count());
        }

        [Fact]
        public async Task GuestProfile_EmailIsNull_SetsEmptyString()
        {
            _profileServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(Result<Users>.Success(new Users { Email = null }));
            _postServiceMock.Setup(s => s.GetPostsByUserIdAsync(1)).ReturnsAsync(Result<IEnumerable<PostCardDto>>.Success(new List<PostCardDto>()));

            var result = await _controller.GuestProfile(1);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProfileViewModel>(viewResult.Model);
            Assert.Equal(string.Empty, model.Email);
        }
    }
}