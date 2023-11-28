using Dapper;
using Npgsql;

namespace EventManagementService.Test.Shared;

public class TestDataContext
{
    public string ConnectionString { get; set; } =
        "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres";

    public async Task Clean()
    {
        var dropStatement = """
                            TRUNCATE event, event_attendee, image RESTART IDENTITY CASCADE;
                            """;


        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(dropStatement);
        await connection.CloseAsync();
    }
}