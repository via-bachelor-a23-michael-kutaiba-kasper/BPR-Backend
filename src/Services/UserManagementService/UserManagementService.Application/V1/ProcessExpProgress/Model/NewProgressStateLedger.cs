using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model;

public class ExperienceGainedLedger
{
    private readonly IDictionary<string, IExpGeneratingEvent> _ledger = new Dictionary<string, IExpGeneratingEvent>();

    public void RegisterExpGeneratingEvent(string userId, ExpGeneratingEventType type, object? arg = null)
    {
        if (!_ledger.ContainsKey(userId))
        {
            // Start experience gain at 0
            _ledger.Add(userId, new IdentityDecorator(null!, 0));
        }

        var currentDecorator = _ledger[userId];
        IExpGeneratingEvent updatedDecorator = type switch
        {
            ExpGeneratingEventType.EventJoined => new EventJoinedEvent(currentDecorator),
            ExpGeneratingEventType.EventReviewed => new EventReviewedEvent(currentDecorator, (Review) arg),
            ExpGeneratingEventType.HostEvent => new HostEventEvent(currentDecorator, (IReadOnlyCollection<Event>) arg),
            ExpGeneratingEventType.JoinEvent => new JoinEventEvent(currentDecorator),
            ExpGeneratingEventType.RateEvent => new RateEventEvent(currentDecorator, (IReadOnlyCollection<Review>) arg),
            ExpGeneratingEventType.SurveyCompleted => new SurveyCompletedEvent(currentDecorator),
            _ => throw new Exception("Unknown event")
        };

        _ledger[userId] = updatedDecorator;
    }

    public long GetExperienceGained(string userId)
    {
        return _ledger.ContainsKey(userId) ? 0 : _ledger[userId].GetExperienceGained();
    }
}