using System.Reflection;
using Dapper;
using DbUp;
using Npgsql;
using UserManagementService.Infrastructure;

// var connectionString = "Server=34.159.177.93;Port=5432;Database=postgres;User Id=postgres;Password=postgres";
// var connectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres";
var connectionStringManager = new ConnectionStringManager();
var connectionString = connectionStringManager.GetConnectionString();
var connection = new NpgsqlConnection(connectionString);
connection.Open();
connection.Execute("CREATE SCHEMA IF NOT EXISTS user_progress; SET SCHEMA 'user_progress';");
connection.Close();
Console.WriteLine(connectionString);
EnsureDatabase.For.PostgresqlDatabase(connectionString);
var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
        .JournalToPostgresqlTable("user_progress", "schemaversions")
        .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
        .LogToConsole()
        .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(result.Error);
    Console.ResetColor();
#if DEBUG
    Console.ReadLine();
#endif
    return -1;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();

return 0;