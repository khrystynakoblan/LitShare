using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Підключення до бази даних успішне\n");

                PrintTable(connection, "users");
                PrintTable(connection, "posts");
                PrintTable(connection, "genres");
                PrintTable(connection, "books_genres");
                PrintTable(connection, "complaints");

                SeedDatabase(connection);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }


    static void PrintTable(NpgsqlConnection connection, string tableName)
    {
        Console.WriteLine($"Дані з таблиці: {tableName}");
        string query = $"SELECT * FROM {tableName} LIMIT 20;";

        using (var cmd = new NpgsqlCommand(query, connection))
        using (var reader = cmd.ExecuteReader())
        {
            if (!reader.HasRows)
            {
                Console.WriteLine("У таблиці немає даних\n");
                return;
            }

            for (int i = 0; i < reader.FieldCount; i++)
                Console.Write($"{reader.GetName(i),-25}");
            Console.WriteLine("\n" + new string('-', 210));

            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var value = reader[i];
                    if (reader.GetName(i).ToLower() == "password")
                        value = value.ToString().Length > 10 ? value.ToString().Substring(0, 10) + "..." : value;

                    Console.Write($"{value,-25}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    static void SeedDatabase(NpgsqlConnection connection)
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

        var genreNames = new[] { "Роман", "Детектив", "Фантастика", "Історія", "Поезія",
                             "Драма", "Комедія", "Трилер", "Фентезі", "Класика" };
        for (int i = 0; i < GENRES_COUNT; i++)
        {
            string insertGenre = "INSERT INTO genres (name) VALUES (@name)";
            using var cmd = new NpgsqlCommand(insertGenre, connection);
            cmd.Parameters.AddWithValue("name", genreNames[i]);
            cmd.ExecuteNonQuery();
        }

        for (int i = 0; i < POSTS_COUNT; i++)
        {
            int userId = rand.Next(1, USERS_COUNT + 1);
            string title = $"Книга_{i + 1}";
            string author = $"Автор_{rand.Next(1, 20)}";
            string dealType = (rand.Next(2) == 0) ? "exchange" : "donation";
            string description = $"Опис книги {i + 1}.";
            string photo = $"https://photo{i + 1}";

            string insertPost = @"
            INSERT INTO posts (user_id, title, author, deal_type, description, photo_url)
            VALUES (@uid, @title, @auth, @deal::deal_type_t, @desc, @photo);
        ";
            using var cmd = new NpgsqlCommand(insertPost, connection);
            cmd.Parameters.AddWithValue("uid", userId);
            cmd.Parameters.AddWithValue("title", title);
            cmd.Parameters.AddWithValue("auth", author);
            cmd.Parameters.AddWithValue("deal", dealType);
            cmd.Parameters.AddWithValue("desc", description);
            cmd.Parameters.AddWithValue("photo", (object)photo ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }

        for (int postId = 1; postId <= POSTS_COUNT; postId++)
        {
            int genresPerBook = rand.Next(1, 4); 
            var usedGenres = new HashSet<int>();

            for (int j = 0; j < genresPerBook; j++)
            {
                int genreId;
                do
                {
                    genreId = rand.Next(1, GENRES_COUNT + 1);
                } while (usedGenres.Contains(genreId));

                usedGenres.Add(genreId);

                string insertRel = "INSERT INTO books_genres (post_id, genre_id) VALUES (@pid, @gid);";
                using var cmd = new NpgsqlCommand(insertRel, connection);
                cmd.Parameters.AddWithValue("pid", postId);
                cmd.Parameters.AddWithValue("gid", genreId);
                cmd.ExecuteNonQuery();
            }
        }

        for (int i = 0; i < COMPLAINTS_COUNT; i++)
        {
            int postId = rand.Next(1, POSTS_COUNT + 1);
            int userId = rand.Next(1, USERS_COUNT + 1);
            string text = $"Скарга {i + 1} на пост {postId}.";

            string insertComplaint = @"
            INSERT INTO complaints (text, post_id, complainant_id)
            VALUES (@txt, @pid, @uid);
        ";
            using var cmd = new NpgsqlCommand(insertComplaint, connection);
            cmd.Parameters.AddWithValue("txt", text);
            cmd.Parameters.AddWithValue("pid", postId);
            cmd.Parameters.AddWithValue("uid", userId);
            cmd.ExecuteNonQuery();
        }

        Console.WriteLine($"Створено {USERS_COUNT} користувачів");
        Console.WriteLine($"Створено {GENRES_COUNT} жанрів");
        Console.WriteLine($"Створено {POSTS_COUNT} постів");
        Console.WriteLine($"Створено скарги");
    }

}
