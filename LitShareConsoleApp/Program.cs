using Microsoft.Extensions.Configuration;
using LitShareConsoleApp.DAL;
using LitShareConsoleApp.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var dbService = new DatabaseService(config);
var menu = new MainMenu(dbService);
menu.Show();