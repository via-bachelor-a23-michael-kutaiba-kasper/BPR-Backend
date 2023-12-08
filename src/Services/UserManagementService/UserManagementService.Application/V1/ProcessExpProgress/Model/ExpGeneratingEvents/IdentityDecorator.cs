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
        if (base._event is null)
        {
            return _identity;
        }

        return _identity + base.GetExperienceGained();
    }
}