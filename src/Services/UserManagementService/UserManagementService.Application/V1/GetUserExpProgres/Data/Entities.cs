namespace UserManagementService.Application.V1.GetUserExpProgres.Data;

public class LevelEntity
{
    public int id{ get; set; }
    public long min_exp { get; set; }
    public long max_exp{ get; set; }
    public string name{ get; set; }
}