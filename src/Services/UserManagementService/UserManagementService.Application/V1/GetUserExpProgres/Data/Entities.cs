namespace UserManagementService.Application.V1.GetUserExpProgres.Data;

public class UserExpProgressEntity
{
    public int id{ get; set; }
    public string user_id{ get; set; }
    public long exp_gained{ get; set; }
    public DateTimeOffset datetime{ get; set; }
}

public class UserStatsHistoryEntity
{
    public int id{ get; set; }
    public string user_id{ get; set; }
    public int reviews_created{ get; set; }
    public int events_hosted{ get; set; }
    public DateTimeOffset datetime{ get; set; }
}

public class ProgressEntity
{
    public int id{ get; set; }
    public string user_id{ get; set; }
    public long total_exp{ get; set; }
    public int stage{ get; set; }
}

public class LevelEntity
{
    public int id{ get; set; }
    public long min_exp { get; set; }
    public long max_exp{ get; set; }
    public string name{ get; set; }
}