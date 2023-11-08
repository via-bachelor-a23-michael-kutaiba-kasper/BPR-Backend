using System.Reflection;
using DbUp;

// var connectionString = "Server=34.159.177.93;Port=5432;Database=postgres;User Id=postgres;Password=postgres";
var connectionString = "Server=eventmanagement_postgres;Port=5432;Database=postgres;User Id=postgres;Password=postgres";
EnsureDatabase.For.PostgresqlDatabase(connectionString);
var upgrader =
    DeployChanges.To
        .PostgresqlDatabase(connectionString)
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