using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public record ProcessExpProgressRequest() : IRequest;

public class ProcessExpProgressHandler : IRequestHandler<ProcessExpProgressRequest>
{
    private readonly ExperienceGainedLedger _ledger = new ExperienceGainedLedger();
    private readonly ILogger<ProcessExpProgressHandler> _logger;
    private readonly IEventsRepository _eventsRepository;
    private readonly IAttendeesRepository _attendeesRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IInterestSurveyRepository _surveyRepository;

    public ProcessExpProgressHandler(ILogger<ProcessExpProgressHandler> logger, IEventsRepository eventsRepository,
        IAttendeesRepository attendeesRepository, IReviewRepository reviewRepository,
        IInterestSurveyRepository surveyRepository)
    {
        _logger = logger;
        _eventsRepository = eventsRepository;
        _attendeesRepository = attendeesRepository;
        _reviewRepository = reviewRepository;
        _surveyRepository = surveyRepository;
    }

    public async Task Handle(ProcessExpProgressRequest request, CancellationToken cancellationToken)
    {
        var newlyCreatedEvents = await _eventsRepository.GetNewlyCreatedEvents();
        foreach (var newlyCreatedEvent in newlyCreatedEvents)
        {
            var hostedEvents =
                (await _eventsRepository.GetHostedEvents(newlyCreatedEvent.Host.UserId))
                .Where(e => e.Id != newlyCreatedEvent.Id).ToList();

            _ledger.RegisterExpGeneratingEvent(newlyCreatedEvent.Host.UserId, ExpGeneratingEventType.HostEvent,
                hostedEvents);
        }

        var attendances = await _attendeesRepository.GetNewEventAttendees();
        foreach (var attendance in attendances)
        {
            _ledger.RegisterExpGeneratingEvent(attendance.Event.Host.UserId, ExpGeneratingEventType.EventJoined);
            _ledger.RegisterExpGeneratingEvent(attendance.UserId, ExpGeneratingEventType.EventJoined);
        }

        var newReviews = await _reviewRepository.GetNewReviews();
        foreach (var newReview in newReviews)
        {
            var previousReviews = (await _reviewRepository.GetReviewsByUser(newReview.ReviewerId))
                .Where(review => review.EventId != newReview.EventId).ToList();
            _ledger.RegisterExpGeneratingEvent(newReview.ReviewerId, ExpGeneratingEventType.RateEvent, previousReviews);

            var reviewedEvent = await _eventsRepository.GetById(newReview.EventId);
            _ledger.RegisterExpGeneratingEvent(reviewedEvent.Host.UserId, ExpGeneratingEventType.EventReviewed,
                newReview);
        }

        var newUsersThatHasCompletedSurveys = await _surveyRepository.GetNewlyCreatedSurveyUserList();
        foreach (var userId in newUsersThatHasCompletedSurveys)
        {
            _ledger.RegisterExpGeneratingEvent(userId, ExpGeneratingEventType.SurveyCompleted);
        }

        throw new NotImplementedException();
    }
}