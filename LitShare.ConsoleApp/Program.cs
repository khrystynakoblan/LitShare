using System;
using System.IO;
using System.Threading.Tasks;

namespace LitShare.ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var fakeBooks = new FakeBookService();
            var fakeUsers = new FakeUserService();
            var fakeComplaints = new FakeComplaintsService();

            await RunAsync(Console.In, Console.Out, fakeBooks, fakeUsers, fakeComplaints);
        }

        public static async Task RunAsync(
            TextReader input,
            TextWriter output,
            IFakeBookService books,
            IFakeUserService users,
            IFakeComplaintsService complaints)
        {
            try
            {
                output.WriteLine("База підключена успішно");

                output.WriteLine("1. Вивести всі книги");
                output.WriteLine("2. Вивести всі жанри");
                output.WriteLine("3. Додати нового користувача");
                output.WriteLine("4. Перевірити логін");
                output.WriteLine("5. Додати скаргу");
                output.Write("Ваш вибір: ");

                string choice = input.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        var allBooks = await books.GetAllBooksAsync();
                        foreach (var b in allBooks)
                            output.WriteLine($"{b.Id}: {b.Title} — {b.Author} ({b.Genre})");
                        break;

                    case "2":
                        var genres = await books.GetGenresAsync();
                        output.WriteLine("Жанри:");
                        foreach (var g in genres)
                            output.WriteLine($"- {g}");
                        break;

                    case "3":
                        output.Write("Ім'я: ");
                        string name = input.ReadLine()?.Trim() ?? "";
                        output.Write("Email: ");
                        string email = input.ReadLine()?.Trim() ?? "";
                        output.Write("Телефон: ");
                        string phone = input.ReadLine()?.Trim() ?? "";
                        output.Write("Пароль: ");
                        string pass = input.ReadLine()?.Trim() ?? "";

                        users.AddUser(name, email, phone, pass);
                        output.WriteLine("Користувача додано!");
                        break;

                    case "4":
                        output.Write("Email: ");
                        string login = input.ReadLine()?.Trim() ?? "";
                        output.Write("Пароль: ");
                        string psw = input.ReadLine()?.Trim() ?? "";

                        bool valid = await users.ValidateUser(login, psw);
                        output.WriteLine(valid ? "Авторизація успішна" : "Невірні дані");
                        break;

                    case "5":
                        output.Write("Текст скарги: ");
                        string txt = input.ReadLine()?.Trim() ?? "";

                        int postId = ReadInt(input, output, "ID книги: ");
                        int userId = ReadInt(input, output, "ID скаржника: ");

                        complaints.AddComplaint(txt, postId, userId);
                        output.WriteLine("Скаргу додано!");
                        break;

                    default:
                        output.WriteLine("Невірний вибір");
                        break;
                }

                output.WriteLine("Натисніть будь-яку клавішу для виходу...");
            }
            catch (Exception ex)
            {
                output.WriteLine($"Сталася помилка: {ex.Message}");
            }
        }

        public static int ReadInt(TextReader input, TextWriter output, string prompt)
        {
            int value;
            while (true)
            {
                output.Write(prompt);
                if (int.TryParse(input.ReadLine(), out value))
                    return value;
                output.WriteLine("Невірне число, спробуйте ще раз.");
            }
        }
    }

    // Інтерфейси та фейкові сервіси
    public interface IFakeBookService
    {
        Task<Book[]> GetAllBooksAsync();
        Task<string[]> GetGenresAsync();
    }

    public interface IFakeUserService
    {
        void AddUser(string name, string email, string phone, string pass);
        Task<bool> ValidateUser(string email, string pass);
    }

    public interface IFakeComplaintsService
    {
        void AddComplaint(string text, int postId, int userId);
    }

    public class FakeBookService : IFakeBookService
    {
        public Task<Book[]> GetAllBooksAsync() =>
            Task.FromResult(new[] { new Book { Id = 1, Title = "Book1", Author = "Author1", Genre = "Genre1" } });

        public Task<string[]> GetGenresAsync() => Task.FromResult(new[] { "Genre1", "Genre2" });
    }

    public class FakeUserService : IFakeUserService
    {
        public bool UserAdded { get; private set; } = false;
        public void AddUser(string name, string email, string phone, string pass) => UserAdded = true;
        public Task<bool> ValidateUser(string email, string pass) =>
            Task.FromResult(email == "test@test.com" && pass == "pass");
    }

    public class FakeComplaintsService : IFakeComplaintsService
    {
        public bool ComplaintAdded { get; private set; } = false;
        public void AddComplaint(string text, int postId, int userId) => ComplaintAdded = true;
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
    }
}
