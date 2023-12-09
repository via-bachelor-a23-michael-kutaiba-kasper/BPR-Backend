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
            // Temporary solution for now, because event management is not sending the host correctly.
            // Ideally we would get host directly from the published message in the published event.
            var newEvent = await _eventsRepository.GetById(newlyCreatedEvent.Id);
            var hostedEventsCount =
                await _eventsRepository.GetHostedEventsCount(newEvent.Host.UserId);
            ledger.RegisterExpGeneratingEvent(newEvent.Host.UserId, e => new HostEventEvent(e, hostedEventsCount));
            await _progressRepository.RegisterNewEventsHostedCount(newEvent.Host.UserId, 1);
        }
    }
}