using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.FetchUserAchievements.Exceptions;
using UserManagementService.Application.V1.FetchUserAchievements.Repository;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.FetchUserAchievements;

public record FetchUserAchievementsRequest(string UserId) : IRequest<IReadOnlyCollection<Achievement>>;

public class FetchUserAchievementsHandler
    : IRequestHandler<FetchUserAchievementsRequest, IReadOnlyCollection<Achievement>>
{
    private readonly ISqlUserAchievementsRepository _sqlUserAchievementsRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<FetchUserAchievementsHandler> _logger;

    public FetchUserAchievementsHandler
    (
        ISqlUserAchievementsRepository sqlUserAchievementsRepository,
        ILogger<FetchUserAchievementsHandler> logger,
        IUserRepository userRepository
    )
    {
        _sqlUserAchievementsRepository = sqlUserAchievementsRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyCollection<Achievement>> Handle
    (
        FetchUserAchievementsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            /*var userExists = await _userRepository.UserExists(request.UserId);
            if (!userExists)
            {
                throw new UserNotFoundException($"No user with id {request.UserId} have been found");
            }*/

            var userAchievements = await _sqlUserAchievementsRepository.FetchUserAchievementByUserId(request.UserId);
            _logger.LogInformation(
                $"{JsonSerializer.Serialize(userAchievements)} achievements have been fetch for user: {request.UserId}");
            return userAchievements;
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to fetch user achievements for user: {request.UserId}, {e.StackTrace}");
            throw new FetchUserAchievementsException(
                $"Something went wrong while fetching user achievements for user with id {request.UserId} at: {DateTimeOffset.UtcNow}",
                e);
        }
    }
}