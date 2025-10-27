using Npgsql;

namespace LitShareConsoleApp.BLL
{
    public class DataPrinter
    {
        public void PrintTable(NpgsqlConnection connection, string tableName)
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
    }
}