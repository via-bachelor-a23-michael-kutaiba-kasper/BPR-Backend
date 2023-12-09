namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class IdentityDecorator : ExpGeneratingEventDecorator
{
    private readonly long _identity;

    public IdentityDecorator(IExpGeneratingEvent e, long identity) : base(e)
    {
        _identity = identity;
    }

    public override long GetExperienceGained()
    {
        return _identity + _event?.GetExperienceGained() ?? 0;
    }
}