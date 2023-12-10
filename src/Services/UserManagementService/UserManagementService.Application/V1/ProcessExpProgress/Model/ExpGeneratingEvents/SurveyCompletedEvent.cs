namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class SurveyCompletedEvent : ExpGeneratingEventDecorator
{
    private const int Reward = 100;
    
    public SurveyCompletedEvent(IExpGeneratingEvent e) : base(e)
    {
    }
    
    public override long GetExperienceGained()
    {
        return base.GetExperienceGained() + Reward;
    }
}