namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class NewAchievementEvent: ExpGeneratingEventDecorator
{
    private readonly int _reward;

    public NewAchievementEvent(IExpGeneratingEvent e, int reward): base(e)
    {
        _reward = reward;
    }

    public long GetExperienceGained()
    {
        return (_event?.GetExperienceGained() ?? 0) + _reward;
    }
}