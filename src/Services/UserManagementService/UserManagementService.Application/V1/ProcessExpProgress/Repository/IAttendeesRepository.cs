using Npgsql;
using UserManagementService.Application.V1.ProcessExpProgress.Dtos;
using UserManagementService.Infrastructure;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IAttendeesRepository
{
    public Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees();
}

public class AttendeesRepository : IAttendeesRepository
{
    private readonly IEventBus _eventBus;
    public AttendeesRepository(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }
    
    public async Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees()
    {
        throw new NotImplementedException();
    }
}