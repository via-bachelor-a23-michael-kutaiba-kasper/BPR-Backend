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
    private readonly IProgressRepository _progressRepository;

    public ProcessExpProgressHandler(ILogger<ProcessExpProgressHandler> logger, IEventsRepository eventsRepository,
        IAttendeesRepository attendeesRepository, IReviewRepository reviewRepository,
        IInterestSurveyRepository surveyRepository, IProgressRepository progressRepository)
    {
        _logger = logger;
        _eventsRepository = eventsRepository;
        _attendeesRepository = attendeesRepository;
        _reviewRepository = reviewRepository;
        _surveyRepository = surveyRepository;
        _progressRepository = progressRepository;
    }

    public async Task Handle(ProcessExpProgressRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing experience gains");
        await RegisterNewlyCreatedEventsInLedger();
        await RegisterNewAttendeesInLedger();
        await RegisterNewReviewsInLedger();
        await RegisterNewCompletedSurveysInLedger();
        await CommitNewExperienceGains();
    }

    private async Task RegisterNewCompletedSurveysInLedger()
    {
        _logger.LogInformation("Processing newly completed surveys experience gains");
        var newUsersThatHasCompletedSurveys = await _surveyRepository.GetNewlyCreatedSurveyUserList();
        foreach (var userId in newUsersThatHasCompletedSurveys)
        {
            _ledger.RegisterExpGeneratingEvent(userId, e => new SurveyCompletedEvent(e));
        }
    }

    private async Task RegisterNewReviewsInLedger()
    {
        _logger.LogInformation("Processing reviews experience gains");
        var newReviews = await _reviewRepository.GetNewReviews();
        foreach (var newReview in newReviews)
        {
            var previousReviews = (await _reviewRepository.GetReviewsByUser(newReview.ReviewerId))
                .Where(review => review.EventId != newReview.EventId).ToList();
            _ledger.RegisterExpGeneratingEvent(newReview.ReviewerId, e => new RateEventEvent(e, previousReviews));

            var reviewedEvent = await _eventsRepository.GetById(newReview.EventId);
            _ledger.RegisterExpGeneratingEvent(reviewedEvent.Host.UserId, e => new EventReviewedEvent(e, newReview));
        }
    }

    private async Task RegisterNewAttendeesInLedger()
    {
        _logger.LogInformation("Processing attendees experience gains");
        var attendances = await _attendeesRepository.GetNewEventAttendees();
        foreach (var attendance in attendances)
        {
            _ledger.RegisterExpGeneratingEvent(attendance.Event.Host.UserId, e => new EventJoinedEvent(e));
            _ledger.RegisterExpGeneratingEvent(attendance.UserId, e => new JoinEventEvent(e));
        }
    }

    private async Task RegisterNewlyCreatedEventsInLedger()
    {
        _logger.LogInformation("Processing host event experience gains");
        var newlyCreatedEvents = await _eventsRepository.GetNewlyCreatedEvents();
        foreach (var newlyCreatedEvent in newlyCreatedEvents)
        {
            var hostedEvents =
                (await _eventsRepository.GetHostedEvents(newlyCreatedEvent.Host.UserId))
                .Where(e => e.Id != newlyCreatedEvent.Id).ToList();

            _ledger.RegisterExpGeneratingEvent(newlyCreatedEvent.Host.UserId, e => new HostEventEvent(e, hostedEvents));
        }
    }

    private async Task CommitNewExperienceGains()
    {
        var userIds = _ledger.GetUserIds();
        foreach (var userId in userIds)
        {
            await _progressRepository.AddExpToUserProgressAsync(userId, _ledger.GetExperienceGained(userId));
        }
    }
}