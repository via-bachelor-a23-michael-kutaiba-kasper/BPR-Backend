using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

public class NewlyCreatedEventsStrategy: IExpStrategy
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IProgressRepository _progressRepository;
    
    public NewlyCreatedEventsStrategy(IEventsRepository eventsRepository, IProgressRepository progressRepository)
    {
        _eventsRepository = eventsRepository;
        _progressRepository = progressRepository;
    }

    public async Task Register(ExperienceGainedLedger ledger, ILogger logger)
    {
        
        logger.LogInformation("Processing host event experience gains");
        var newlyCreatedEvents = await _eventsRepository.GetNewlyCreatedEvents();
        foreach (var newlyCreatedEvent in newlyCreatedEvents)
        {
            var hostedEventsCount =
                await _eventsRepository.GetHostedEventsCount(newlyCreatedEvent.Host.UserId);
            ledger.RegisterExpGeneratingEvent(newlyCreatedEvent.Host.UserId, e => new HostEventEvent(e, hostedEventsCount));
            await _progressRepository.RegisterNewEventsHostedCount(newlyCreatedEvent.Host.UserId, 1);
        }
    }
}