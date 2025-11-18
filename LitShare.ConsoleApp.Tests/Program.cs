using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using LitShare.ConsoleApp;

namespace LitShare.Tests
{
    public class ConsoleAppTests
    {
        [Fact]
        public async Task RunAsync_ShowMenu()
        {
            var input = new StringReader("1\n");
            var output = new StringWriter();
            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("База підключена успішно", consoleOutput);
            Assert.Contains("1. Вивести всі книги", consoleOutput);
            Assert.Contains("Натисніть будь-яку клавішу для виходу", consoleOutput);
        }

        [Fact]
        public async Task RunAsync_InvalidChoice()
        {
            var input = new StringReader("9\n");
            var output = new StringWriter();
            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Невірний вибір", consoleOutput);
        }

        [Fact]
        public async Task RunAsync_AddUser()
        {
            var input = new StringReader("3\nTest User\nuser@test.com\n12345\npass\n");
            var output = new StringWriter();
            var fakeUser = new FakeUserService();

            await Program.RunAsync(input, output, new FakeBookService(), fakeUser, new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Користувача додано!", consoleOutput);
            Assert.True(fakeUser.UserAdded);
        }

        [Fact]
        public async Task RunAsync_ShowGenres()
        {
            var input = new StringReader("2\n");
            var output = new StringWriter();
            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Жанри:", consoleOutput);
            Assert.Contains("- Genre1", consoleOutput);
            Assert.Contains("- Genre2", consoleOutput);
        }

        [Fact]
        public async Task RunAsync_ValidLogin()
        {
            var input = new StringReader("4\ntest@test.com\npass\n");
            var output = new StringWriter();
            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Авторизація успішна", consoleOutput);
        }

        [Fact]
        public async Task RunAsync_InvalidLogin()
        {
            var input = new StringReader("4\nwrong@test.com\n1234\n");
            var output = new StringWriter();
            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), new FakeComplaintsService());

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Невірні дані", consoleOutput);
        }

        [Fact]
        public async Task RunAsync_AddComplaint()
        {
            var input = new StringReader("5\nTest complaint\n1\n2\n");
            var output = new StringWriter();
            var complaints = new FakeComplaintsService();

            await Program.RunAsync(input, output, new FakeBookService(), new FakeUserService(), complaints);

            string consoleOutput = output.ToString().Replace("\r", "");
            Assert.Contains("Скаргу додано!", consoleOutput);
            Assert.True(complaints.ComplaintAdded);
        }

        [Fact]
        public void ReadInt_InvalidThenValidInput()
        {
            var input = new StringReader("abc\n5\n");
            var output = new StringWriter();

            int result = Program.ReadInt(input, output, "Enter number: ");
            string consoleOutput = output.ToString().Replace("\r", "");

            Assert.Equal(5, result);
            Assert.Contains("Невірне число", consoleOutput);
        }
    }
}
