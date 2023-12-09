using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class RateEventEvent: ExpGeneratingEventDecorator
{
    private const int ExpPerPreviousReview = 10;
    private const int BaseReward = 100;
    private readonly int _previousReviewsCount;

    public RateEventEvent(IExpGeneratingEvent e, int previousReviewsCount ) : base(e)
    {
        _previousReviewsCount = previousReviewsCount;
    }

    public override long GetExperienceGained()
    {
        return base.GetExperienceGained() + BaseReward + ExpPerPreviousReview * _previousReviewsCount;
    }
}