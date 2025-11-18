using LitShare.BLL.Services;
using LitShare.DAL;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            using var db = new LitShareDbContext();

            var books = new BookService(db);
            var users = new UserService(db);
            var complaints = new ComplaintsService(db);

            Console.WriteLine("База підключена успішно\n");

            Console.WriteLine("1. Вивести всі книги");
            Console.WriteLine("2. Вивести всі жанри");
            Console.WriteLine("3. Додати нового користувача");
            Console.WriteLine("4. Перевірити логін");
            Console.WriteLine("5. Додати скаргу");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    var allBooks = await books.GetAllBooksAsync();
                    foreach (var b in allBooks)
                        Console.WriteLine($"{b.Id}: {b.Title} — {b.Author} ({b.Genre})");
                    break;

                case "2":
                    var genres = await books.GetGenresAsync();
                    Console.WriteLine("Жанри:");
                    genres.ForEach(g => Console.WriteLine($"- {g}"));
                    break;

                case "3":
                    Console.Write("Ім'я: ");
                    string name = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Email: ");
                    string email = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Телефон: ");
                    string phone = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Пароль: ");
                    string pass = Console.ReadLine()?.Trim() ?? "";

                    users.AddUser(name, email, phone, pass, "RegionX", "DistrictX", "CityX");

                    Console.WriteLine("Користувача додано!");
                    break;

                case "4":
                    Console.Write("Email: ");
                    string login = Console.ReadLine()?.Trim() ?? "";
                    Console.Write("Пароль: ");
                    string psw = Console.ReadLine()?.Trim() ?? "";

                    bool valid = await users.ValidateUser(login, psw); 
                    Console.WriteLine(valid ? "Авторизація успішна" : "Невірні дані");
                    break;

                case "5":
                    Console.Write("Текст скарги: ");
                    string txt = Console.ReadLine()?.Trim() ?? "";

                    int postId = ReadInt("ID книги: ");
                    int userId = ReadInt("ID скаржника: ");

                    complaints.AddComplaint(txt, postId, userId);
                    Console.WriteLine("Скаргу додано!");
                    break;

                default:
                    Console.WriteLine("Невірний вибір");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Сталася помилка: {ex.Message}");
        }

        Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
        Console.ReadKey();
    }

    static int ReadInt(string prompt)
    {
        int value;
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out value))
                return value;
            Console.WriteLine("Невірне число, спробуйте ще раз.");
        }
    }
}
