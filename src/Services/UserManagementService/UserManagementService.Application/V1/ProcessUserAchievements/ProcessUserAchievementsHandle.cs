using MediatR;
using UserManagementService.Application.V1.ProcessUserAchievements.Dto;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessUserAchievements;

public record ProcessUserAchievementsRequest(string UserId) : IRequest<ProcessAchievementsDto>;

public class ProcessUserAchievementsHandle : IRequestHandler<ProcessUserAchievementsRequest, ProcessAchievementsDto>
{
    public Task<ProcessAchievementsDto> Handle
    (
        ProcessUserAchievementsRequest request,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}