using System;
using System.IO;
using Xunit;
using LitShare.ConsoleApp;

namespace LitShare.ConsoleApp.Tests
{
    public class InMemoryBookRepositoryTests
    {
        [Fact]
        public void AddUser_ShouldAssignIdAndStoreUser()
        {
            var repo = new InMemoryBookRepository();
            var user = new User { Name = "Test" };
            int id = repo.AddUser(user);

            Assert.Equal(1, id);
            Assert.Single(repo.GetAllUsers());
            Assert.Equal("Test", repo.GetAllUsers()[0].Name);
        }

        [Fact]
        public void AddPost_ShouldAssignIdAndStorePost()
        {
            var repo = new InMemoryBookRepository();
            var post = new Post { Title = "Book" };
            int id = repo.AddPost(post);

            Assert.Equal(1, id);
            Assert.Single(repo.GetAllPosts());
            Assert.Equal("Book", repo.GetAllPosts()[0].Title);
        }

        [Fact]
        public void AddGenre_ShouldAssignIdAndStoreGenre()
        {
            var repo = new InMemoryBookRepository();
            var genre = new Genre { Name = "Fiction" };
            int id = repo.AddGenre(genre);

            Assert.Equal(1, id);
            Assert.Single(repo.GetAllGenres());
            Assert.Equal("Fiction", repo.GetAllGenres()[0].Name);
        }

        [Fact]
        public void AddComplaint_ShouldAssignIdAndStore()
        {
            var repo = new InMemoryBookRepository();
            var complaint = new Complaint { Text = "Bad" };
            int id = repo.AddComplaint(complaint);

            Assert.Equal(1, id);
            Assert.Single(repo.GetAllComplaints());
            Assert.Equal("Bad", repo.GetAllComplaints()[0].Text);
        }

        [Fact]
        public void ClearAllData_ShouldEmptyAllCollections()
        {
            var repo = new InMemoryBookRepository();
            repo.AddUser(new User());
            repo.AddPost(new Post());
            repo.AddGenre(new Genre());
            repo.AddComplaint(new Complaint());

            repo.ClearAllData();

            Assert.Empty(repo.GetAllUsers());
            Assert.Empty(repo.GetAllPosts());
            Assert.Empty(repo.GetAllGenres());
            Assert.Empty(repo.GetAllComplaints());
        }
    }

    public class DataSeederTests
    {
        [Fact]
        public void SeedUsersAndPosts_ShouldAddCorrectNumber()
        {
            var repo = new InMemoryBookRepository();
            var seeder = new DataSeeder(repo);

            seeder.SeedUsersAndPosts(2, 3);

            Assert.Equal(2, repo.GetAllUsers().Count);
            Assert.Equal(6, repo.GetAllPosts().Count);
        }

        [Fact]
        public void SeedGenres_ShouldAddCorrectNumber()
        {
            var repo = new InMemoryBookRepository();
            var seeder = new DataSeeder(repo);

            seeder.SeedGenres(3);

            Assert.Equal(3, repo.GetAllGenres().Count);
        }

        [Fact]
        public void SeedComplaints_ShouldAddCorrectNumber()
        {
            var repo = new InMemoryBookRepository();
            var seeder = new DataSeeder(repo);

            seeder.SeedUsersAndPosts(2, 2);
            seeder.SeedComplaints(4);

            Assert.Equal(4, repo.GetAllComplaints().Count);
        }

        [Fact]
        public void RunMenu_ShouldWorkWithoutErrors()
        {
            var input = new StringReader("1\n2\n3\n4\n0\n");
            var output = new StringWriter();

            ConsoleApp.RunMenu(input, output);

            string consoleOutput = output.ToString();
            Assert.Contains("UserID:", consoleOutput);
            Assert.Contains("PostID:", consoleOutput);
            Assert.Contains("GenreID:", consoleOutput);
            Assert.Contains("ComplaintID:", consoleOutput);
        }
    }
}
