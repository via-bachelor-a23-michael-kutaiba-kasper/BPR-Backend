using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;

public class EventReviewedEvent : ExpGeneratingEventDecorator
{
    private const int GoodReviewReward = 50;
    private const int BadReviewReward = 10;
    private const int VeryBadReviewReward = 0;
    private readonly Review _review;

    public EventReviewedEvent(IExpGeneratingEvent e, Review review) : base(e)
    {
        _review = review;
    }

    public override long GetExperienceGained()
    {
        var reward = _review.Rate switch
        {
            <=0 => VeryBadReviewReward,
            > 3 => GoodReviewReward,
            >= 1 and <= 3 => BadReviewReward,
            _ => 0
        };

        return (_event?.GetExperienceGained()?? 0) + reward;
    }
}