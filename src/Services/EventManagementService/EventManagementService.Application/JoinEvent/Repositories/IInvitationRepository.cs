using EventManagementService.Domain.Models.Events;

namespace EventManagementService.Application.JoinEvent.Repositories;

// TODO: Implement this when we have modelled invitations
public interface IInvitationRepository
{
    public Task<IReadOnlyCollection<Invitation>> GetInvitationsAsync(int eventId);
    public Task AcceptInvitationAsync (Invitation invitation);
}

public class InvitationRepository : IInvitationRepository
{
    public async Task<IReadOnlyCollection<Invitation>> GetInvitationsAsync(int eventId)
    {
        return new List<Invitation>();
    }

    public async Task AcceptInvitationAsync(Invitation invitation)
    {
        // Do nothing for now.
    }
}