using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IEventsRepository
{
    public Task<IReadOnlyCollection<Event>> GetNewlyCreatedEvents();
    public Task<Event> GetById(int eventId);
    public Task<int> GetHostedEventsCount(string userId);
}