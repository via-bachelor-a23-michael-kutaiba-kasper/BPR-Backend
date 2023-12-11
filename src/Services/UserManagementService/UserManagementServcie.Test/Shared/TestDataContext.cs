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
                            TRUNCATE
                                user_progress.timed_criteria,
                                user_progress.category_attendance_criteria,
                                user_progress.monthly_exp_goal,
                                user_progress.progress,
                                user_progress.user_achievement,
                                user_progress.unlockable_achievement_progress,
                                user_progress.unlockable_monthly_goal_progress,
                                user_progress.user_exp_progress,
                                user_progress.user_stats_history
                                RESTART IDENTITY CASCADE;
                            """;


        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await connection.ExecuteAsync(dropStatement);
        await connection.CloseAsync();
    }
}