namespace UserManagementService.Application.V1.ProcessExpProgress.Data;

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
