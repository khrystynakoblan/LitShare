using Xunit;
using LitShare.DAL;
using LitShare.DAL.Models;
using LitShare.BLL.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LitShare.BLL.Tests
{
    public class UserServiceTests
    {
        private LitShareDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LitShareDbContext(options);
        }

        [Fact]
        public void AddUser_Should_SaveUserToDatabase()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("Test User", "test@example.com", "123456789", "password123", "Region", "District", "City");

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
        public void GetUserById_Should_Return_Correct_User()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("User A", "a@example.com", "1", "x", "R", "D", "C");
            var added = context.Users.First();

            var user = service.GetUserById(added.id);
            Assert.NotNull(user);
            Assert.Equal("a@example.com", user.email);
        }

        [Fact]
        public void GetUserProfileById_Should_Return_User_With_Posts()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            var user = new Users { name = "User", email = "u@u.com", phone = "1", password = "x", region = "R", district = "D", city = "C" };
            context.Users.Add(user);
            context.posts.Add(new Posts { title = "Book", author = "A", description = "D", user_id = user.id, User = user });
            context.SaveChanges();

            var result = service.GetUserProfileById(user.id);
            Assert.NotNull(result);
            Assert.Single(result.posts);
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
        public void UpdateUser_Should_Change_User_Data()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("EditUser", "edit@example.com", "123", "pass", "OldRegion", "OldDistrict", "OldCity");
            var existing = context.Users.First();

            var updated = new Users
            {
                id = existing.id,
                region = "NewRegion",
                district = "NewDistrict",
                city = "NewCity",
                phone = "999",
                about = "Updated",
                photo_url = "url.jpg"
            };

            service.UpdateUser(updated);

            var reloaded = context.Users.Find(existing.id);
            Assert.Equal("NewCity", reloaded.city);
            Assert.Equal("999", reloaded.phone);
            Assert.Equal("url.jpg", reloaded.photo_url);
        }

        [Fact]
        public void DeleteUser_Should_Remove_User_And_Posts()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            service.AddUser("DelUser", "delete@example.com", "000", "pass", "R", "D", "C");
            var user = context.Users.First();
            context.posts.Add(new Posts { title = "Book", author = "Me", description = "Desc", user_id = user.id, User = user });
            context.SaveChanges();

            service.DeleteUser(user.id);

            Assert.Empty(context.Users);
            Assert.Empty(context.posts);
        }

        [Fact]
        public void DeleteUser_Should_Throw_When_User_NotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new UserService(context);

            var ex = Assert.Throws<Exception>(() => service.DeleteUser(999));
            Assert.Contains("Користувача не знайдено", ex.Message);
        }

        [Fact]
        public void Default_Constructor_Should_Work()
        {
            // Просто перевіряє, що безконтекстний конструктор не падає
            var service = new UserService();
            Assert.NotNull(service);
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