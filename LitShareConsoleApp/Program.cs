using System;
using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

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
                    Console.Write($"{reader[i],-25}");
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
