using LitShare.BLL.Services;
using LitShare.DAL.Models;
using LitShare.DAL;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LitShare.BLL.Tests
{
    public class ComplaintsServiceTests
    {
        private LitShareDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new LitShareDbContext(options);
        }
        private LitShareDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<LitShareDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LitShareDbContext(options);
        }

        [Fact]
        public void AddComplaint_Should_Save_To_Database()
        {
            var context = GetDbContext();

            var user = new Users
            {
                name = "User",
                email = "mail@test.com",
                phone = "123",
                password = "p",
                region = "R",
                district = "D",
                city = "C"
            };
            var post = new Posts
            {
                title = "Book",
                author = "Author",
                description = "Desc",
                deal_type = DealType.Exchange,
                user_id = user.id,
                User = user
            };

            context.Users.Add(user);
            context.posts.Add(post);
            context.SaveChanges();

            var service = new ComplaintsService(context);

            service.AddComplaint("Reason text", post.id, user.id);

            var complaints = context.complaints.ToList();
            Assert.Single(complaints);
            Assert.Equal("Reason text", complaints.First().text);
            Assert.Equal(post.id, complaints.First().post_id);
        }

        [Fact]
        public void GetAllComplaints_Should_Return_Details()
        {
            // Arrange
            var context = GetDbContext();

            var user = new Users
            {
                name = "User",
                email = "m",
                phone = "1",
                password = "x",
                region = "r",
                district = "d",
                city = "c"
            };
            var post = new Posts
            {
                title = "Book",
                author = "A",
                description = "D",
                deal_type = DealType.Exchange,
                user_id = user.id,
                User = user
            };
            context.Users.Add(user);
            context.posts.Add(post);
            context.SaveChanges();

            var complaint = new Complaints
            {
                text = "Bad",
                post_id = post.id,
                complainant_id = user.id,
                date = DateTime.Now,
                Post = post,
                Complainant = user
            };
            context.complaints.Add(complaint);
            context.SaveChanges();

            var service = new ComplaintsService(context);

            var result = service.GetAllComplaints();

            Assert.Single(result);
            Assert.Equal("Bad", result[0].Text);
            Assert.Equal("Book", result[0].BookTitle);
            Assert.Equal("User", result[0].UserName);
        }

        [Fact]
        public void DeleteComplaint_Should_Remove_Record()
        {
            var context = GetDbContext();

            var user = new Users
            {
                name = "U",
                email = "x@y.com",
                phone = "1",
                password = "x",
                region = "R",
                district = "D",
                city = "Kyiv"
            };
            var post = new Posts
            {
                title = "B",
                author = "A",
                description = "D",
                deal_type = DealType.Exchange,
                user_id = user.id,
                User = user
            };
            context.Users.Add(user);
            context.posts.Add(post);
            context.SaveChanges();

            var complaint = new Complaints
            {
                text = "Delete me",
                date = DateTime.Now,
                post_id = post.id,
                complainant_id = user.id,
                Post = post,
                Complainant = user
            };
            context.complaints.Add(complaint);
            context.SaveChanges();

            var service = new ComplaintsService(context);

            
            service.DeleteComplaint(complaint.id);

            Assert.Empty(context.complaints);
        }
        [Fact]
        public void GetComplaintWithDetails_Should_Return_Complaint_With_Post()
        {
            var context = GetDbContext();

            var post = new Posts { title = "Book", author = "A", description = "D", deal_type = DealType.Exchange };
            context.posts.Add(post);
            context.SaveChanges();

            var complaint = new Complaints { text = "Bad", post_id = post.id, date = DateTime.Now, Post = post };
            context.complaints.Add(complaint);
            context.SaveChanges();

            var service = new ComplaintsService(context);
            var result = service.GetComplaintWithDetails(complaint.id);

            Assert.NotNull(result);
            Assert.Equal("Bad", result.text);
            Assert.Equal("Book", result.Post.title);
        }
        [Fact]
        public void GetComplaintWithDetails_Should_ReturnNull_When_NotFound()
        {
            using var context = CreateInMemoryContext();
            var service = new ComplaintsService(context);

            var result = service.GetComplaintWithDetails(999);

            Assert.Null(result);
        }
    }
}