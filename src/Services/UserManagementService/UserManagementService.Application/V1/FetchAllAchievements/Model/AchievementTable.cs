namespace UserManagementService.Application.V1.FetchAllAchievements.Model;

public class AchievementTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public long reward { get; set; }
    public string icon { get; set; }
}