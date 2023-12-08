using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class HostEventEvent : ExpGeneratingEventDecorator
{
    private const int ExpPerPreviousHostedEvent = 25;
    private const int BaseReward = 50;
    private readonly IReadOnlyCollection<Event> _events;

    public HostEventEvent(IExpGeneratingEvent e, IReadOnlyCollection<Event> events) : base(e)
    {
        _events = events;
    }

    public override long GetExperienceGained()
    {
        return base.GetExperienceGained()  + BaseReward + ExpPerPreviousHostedEvent * _events.Count;
    }
}