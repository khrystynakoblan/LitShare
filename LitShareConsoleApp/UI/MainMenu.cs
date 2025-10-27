using LitShareConsoleApp.DAL;
using LitShareConsoleApp.BLL;

namespace LitShareConsoleApp.UI
{
    public class MainMenu
    {
        private readonly DatabaseService _dbService;
        private readonly DataPrinter _printer;
        private readonly DataSeeder _seeder;

        public MainMenu(DatabaseService dbService)
        {
            _dbService = dbService;
            _printer = new DataPrinter();
            _seeder = new DataSeeder();
        }

        public void Show()
        {
            while (true)
            {
                Console.WriteLine("1. Переглянути таблицю користувачів");
                Console.WriteLine("2. Заповнити базу даних (Seed)");
                Console.WriteLine("3. Вийти");
                Console.Write("Оберіть опцію: ");

                var choice = Console.ReadLine();
                using var conn = _dbService.GetConnection();

                switch (choice)
                {
                    case "1":
                        _printer.PrintTable(conn, "users");
                        break;
                    case "2":
                        _seeder.SeedDatabase(conn);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Невірна опція!");
                        break;
                }
            }
        }
    }
}