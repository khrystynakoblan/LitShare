using Xunit;
using LitShare.DAL;
using LitShare.DAL.Models;
using LitShare.BLL.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LitShare.BLL.Tests
{
    public class UserServiceTests
    {
        private LitShareDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            return new LitShareDbContext(options);
        }

        [Fact]
        public void AddUser_Should_SaveUserToDatabase()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser(
                name: "Test User",
                email: "test@example.com",
                phone: "123456789",
                password: "password123",
                region: "Region",
                district: "District",
                city: "City"
            );

            var user = context.Users.SingleOrDefault(u => u.email == "test@example.com");

            Assert.NotNull(user);
            Assert.Equal("Test User", user.name);
        }

        [Fact]
        public async Task ValidateUser_Should_ReturnTrue_ForCorrectCredentials()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("Login User", "login@example.com", "000", "mypassword", "R", "D", "C");
            var result = await service.ValidateUser("login@example.com", "mypassword");

            Assert.True(result);
        }

        [Fact]
        public void GetAllUsers_Should_Return_All_Users()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("User1", "user1@example.com", "111", "pass1", "R", "D", "C");
            service.AddUser("User2", "user2@example.com", "222", "pass2", "R", "D", "C");

            var users = service.GetAllUsers();

            Assert.Equal(2, users.Count);
        }

        [Fact]
        public void UpdateUserPhoto_Should_Update_PhotoUrl()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("PhotoUser", "photo@example.com", "000", "pass", "R", "D", "C");
            var user = context.Users.First();

            service.UpdateUserPhoto(user.id, "newphoto.jpg");

            var updated = context.Users.First();
            Assert.Equal("newphoto.jpg", updated.photo_url);
        }

        [Fact]
        public void DeleteUser_Should_Remove_User_And_Posts()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("DelUser", "delete@example.com", "000", "pass", "R", "D", "C");
            var user = context.Users.First();

            context.posts.Add(new Posts
            {
                title = "Book",
                author = "Me",
                description = "Desc",
                user_id = user.id
            });
            context.SaveChanges();

            service.DeleteUser(user.id);

            Assert.Empty(context.Users);
            Assert.Empty(context.posts);
        }
        [Fact]
        public async Task ValidateUser_Should_ReturnFalse_For_WrongPassword()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("WrongUser", "wrong@example.com", "000", "password123", "R", "D", "C");
            var result = await service.ValidateUser("wrong@example.com", "incorrect");

            Assert.False(result);
        }
        [Fact]
        public async Task ValidateUser_Should_ReturnFalse_For_NonExisting_Email()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            var result = await service.ValidateUser("ghost@example.com", "whatever");

            Assert.False(result);
        }
    }
}