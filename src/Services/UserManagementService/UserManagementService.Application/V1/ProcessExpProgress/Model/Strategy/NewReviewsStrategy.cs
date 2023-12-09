using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

public class NewReviewsStrategy: IExpStrategy
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IEventsRepository _eventsRepository;
    private readonly IProgressRepository _progressRepository;
    
    public NewReviewsStrategy(IReviewRepository reviewRepository, IEventsRepository eventsRepository, IProgressRepository progressRepository)
    {
        _reviewRepository = reviewRepository;
        _eventsRepository = eventsRepository;
        _progressRepository = progressRepository;
    }
    
    public async Task Register(ExperienceGainedLedger ledger, ILogger logger)
    {
        logger.LogInformation("Processing reviews experience gains");
        var newReviews = await _reviewRepository.GetNewReviews();
        foreach (var newReview in newReviews)
        {
            // minus 1, because we don't want to provide bonus for the new review / new review should nto be counted as old review.
            var previousReviewsCount = await _reviewRepository.GetReviewsCountByUser(newReview.ReviewerId) - 1;
            ledger.RegisterExpGeneratingEvent(newReview.ReviewerId, e => new RateEventEvent(e, previousReviewsCount));
            await _progressRepository.RegisterNewReviewCount(newReview.ReviewerId, 1);

            var reviewedEvent = await _eventsRepository.GetById(newReview.EventId);
            ledger.RegisterExpGeneratingEvent(reviewedEvent.Host.UserId, e => new EventReviewedEvent(e, newReview));
        }
    }
}