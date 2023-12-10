using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

public class NewAttendeesStrategy: IExpStrategy
{
    private readonly IAttendeesRepository _attendeesRepository;
    
    public NewAttendeesStrategy(IAttendeesRepository attendeesRepository)
    {
        _attendeesRepository = attendeesRepository;
    }
    
    public async Task Register(ExperienceGainedLedger ledger, ILogger logger)
    {
        logger.LogInformation("Processing attendees experience gains");
        var attendances = await _attendeesRepository.GetNewEventAttendees();
        foreach (var attendance in attendances)
        {
            ledger.RegisterExpGeneratingEvent(attendance.Event.Host.UserId, e => new EventJoinedEvent(e));
            ledger.RegisterExpGeneratingEvent(attendance.UserId, e => new JoinEventEvent(e));
        }
    }
}