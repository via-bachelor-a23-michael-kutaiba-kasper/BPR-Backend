using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.FetchAllAchievements.Exceptions;
using UserManagementService.Application.V1.FetchAllAchievements.Model;
using UserManagementService.Application.V1.FetchAllAchievements.Repository;

namespace UserManagementService.Application.V1.FetchAllAchievements;

public record FetchAllAchievementsRequest() : IRequest<IReadOnlyCollection<AchievementTable>>;

public class FetchAllAchievementsHandler
    : IRequestHandler<FetchAllAchievementsRequest, IReadOnlyCollection<AchievementTable>>
{
    private readonly ISqlAchievementsRepository _sqlAchievementsRepository;
    private readonly ILogger<FetchAllAchievementsHandler> _logger;

    public FetchAllAchievementsHandler
    (
        ISqlAchievementsRepository sqlAchievementsRepository,
        ILogger<FetchAllAchievementsHandler> logger
    )
    {
        _sqlAchievementsRepository = sqlAchievementsRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<AchievementTable>> Handle
    (
        FetchAllAchievementsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var achievements = await _sqlAchievementsRepository.FetchAllAchievementsInTheSystem();
            _logger.LogInformation(
                $"{achievements.Count} achievements have been successfully fetched at {DateTimeOffset.UtcNow}");
            return achievements;
        }
        catch (Exception e)
        {
            _logger.LogError($"Cannot fetch all achievements at {DateTimeOffset.UtcNow}, {e.StackTrace}");
            throw new FetchAllAchievementsException("Something went wrong while fetching all achievements", e);
        }
    }
}