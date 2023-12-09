namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class JoinEventEvent: ExpGeneratingEventDecorator
{
    private const int Reward = 400;
    
    public JoinEventEvent(IExpGeneratingEvent e) : base(e)
    {
    }

    public override long GetExperienceGained()
    {
        return (_event?.GetExperienceGained() ?? 0) + Reward;
    }
}