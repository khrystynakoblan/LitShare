using Npgsql;
using BCrypt.Net;

namespace LitShareConsoleApp.BLL
{
    public class DataSeeder
    {
        public void SeedDatabase(NpgsqlConnection connection)
        {
            using (var clearCmd = new NpgsqlCommand(@"
                DELETE FROM books_genres;
                DELETE FROM complaints;
                DELETE FROM posts;
                DELETE FROM genres;
                DELETE FROM users;

                ALTER SEQUENCE users_id_seq RESTART WITH 1;
                ALTER SEQUENCE posts_id_seq RESTART WITH 1;
                ALTER SEQUENCE genres_id_seq RESTART WITH 1;
                ALTER SEQUENCE complaints_id_seq RESTART WITH 1;
            ", connection))
            {
                clearCmd.ExecuteNonQuery();
            }

            var rand = new Random();
            const int USERS_COUNT = 40;
            const int GENRES_COUNT = 10;
            const int POSTS_COUNT = 40;
            const int COMPLAINTS_COUNT = 20;

            var cities = new[] { "Львів", "Київ", "Одеса", "Харків", "Дніпро" };
            var regions = new[] { "Львівська", "Київська", "Одеська", "Харківська", "Дніпропетровська" };
            var districts = new[] { "Центральний", "Шевченківський", "Франківський", "Личаківський" };

            for (int i = 0; i < USERS_COUNT; i++)
            {
                int cityIndex = rand.Next(cities.Length);
                string name = $"Користувач_{i + 1}";
                string email = $"user{i + 1}@gmail.com";
                string password = BCrypt.Net.BCrypt.HashPassword($"pass{i + 1}");
                string about = $"Про себе {i + 1}";
                string role = (i % 10 == 0) ? "admin" : "user";

                string insertUser = @"
                    INSERT INTO users (name, email, password, about, role, region, district, city)
                    VALUES (@name, @mail, @pass, @about, @role::role_t, @region, @district, @city);
                ";

                using var cmd = new NpgsqlCommand(insertUser, connection);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("mail", email);
                cmd.Parameters.AddWithValue("pass", password);
                cmd.Parameters.AddWithValue("about", (object)about ?? DBNull.Value);
                cmd.Parameters.AddWithValue("role", role);
                cmd.Parameters.AddWithValue("region", regions[cityIndex]);
                cmd.Parameters.AddWithValue("district", districts[rand.Next(districts.Length)]);
                cmd.Parameters.AddWithValue("city", cities[cityIndex]);
                cmd.ExecuteNonQuery();
            }

            Console.WriteLine($"Створено {USERS_COUNT} користувачів");
        }
    }
}