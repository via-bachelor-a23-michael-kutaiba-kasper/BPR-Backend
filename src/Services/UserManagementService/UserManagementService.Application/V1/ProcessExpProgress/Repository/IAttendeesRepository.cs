using UserManagementService.Application.V1.ProcessExpProgress.Dtos;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IAttendeesRepository
{
    public Task<IReadOnlyCollection<Attendance>> GetNewEventAttendees();
}