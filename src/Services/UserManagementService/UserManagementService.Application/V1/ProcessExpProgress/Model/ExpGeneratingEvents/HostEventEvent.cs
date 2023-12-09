using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class HostEventEvent : ExpGeneratingEventDecorator
{
    private const int ExpPerPreviousHostedEvent = 25;
    private const int BaseReward = 50;
    private readonly int _hostedEventsCount;

    public HostEventEvent(IExpGeneratingEvent e, int hostedEventsCount) : base(e)
    {
        _hostedEventsCount = hostedEventsCount;
    }

    public override long GetExperienceGained()
    {
        return base.GetExperienceGained() + BaseReward + ExpPerPreviousHostedEvent * _hostedEventsCount;
    }
}