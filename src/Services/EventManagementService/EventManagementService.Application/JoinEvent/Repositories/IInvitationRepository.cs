using EventManagementService.Domain.Models.Events;

namespace EventManagementService.Application.JoinEvent.Repositories;

public interface IInvitationRepository
{
    public Task<IReadOnlyCollection<Invitation>> GetInvitationsAsync(int eventId);
    public Task AcceptInvitationAsync (Invitation invitation);
}