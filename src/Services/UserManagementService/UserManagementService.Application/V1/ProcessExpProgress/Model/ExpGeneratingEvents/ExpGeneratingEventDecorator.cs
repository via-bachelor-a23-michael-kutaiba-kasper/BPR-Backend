namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class ExpGeneratingEventDecorator : IExpGeneratingEvent
{
    protected IExpGeneratingEvent _event;
    protected ExpGeneratingEventDecorator(IExpGeneratingEvent e)
    {
        _event = e;
    }


    public virtual long GetExperienceGained()
    {
        return _event.GetExperienceGained();
    }
}