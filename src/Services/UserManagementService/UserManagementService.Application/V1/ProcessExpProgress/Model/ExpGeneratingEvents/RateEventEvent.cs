using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class RateEventEvent: ExpGeneratingEventDecorator
{
    private const int ExpPerPreviousReview = 10;
    private const int BaseReward = 100;
    private readonly IReadOnlyCollection<Review> _reviews;

    public RateEventEvent(IExpGeneratingEvent e, IReadOnlyCollection<Review> reviews) : base(e)
    {
        _reviews = reviews;
    }

    public override long GetExperienceGained()
    {
        return base.GetExperienceGained() + BaseReward + ExpPerPreviousReview * _reviews.Count;
    }
}