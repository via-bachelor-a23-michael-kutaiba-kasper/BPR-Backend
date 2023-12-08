using Dapper;
using Npgsql;

namespace UserManagementServcie.Test.Shared;

public class TestDataContext
{
    public string ConnectionString { get; set; } =
        "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres";

    public async Task Clean()
    {
        // Insert names of tables you want to clean after test
        var dropStatement = """
                            TRUNCATE achievement, timed_criteria, unlockable_progress, category_attendance_criteria, monthly_exp_goal, progress, user_achievement, unlockable_achievement_progress, unlockable_monthly_goal_progress RESTART IDENTITY CASCADE;
                            """;


        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(dropStatement);
        await connection.CloseAsync();
    }
}