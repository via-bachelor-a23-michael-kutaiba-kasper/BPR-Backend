namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class EventJoinedEvent: ExpGeneratingEventDecorator
{
    private const int Reward = 400;
    
    public EventJoinedEvent(IExpGeneratingEvent e) : base(e)
    {
    }

    public override long GetExperienceGained()
    {
        return base.GetExperienceGained() + Reward;
    }
}