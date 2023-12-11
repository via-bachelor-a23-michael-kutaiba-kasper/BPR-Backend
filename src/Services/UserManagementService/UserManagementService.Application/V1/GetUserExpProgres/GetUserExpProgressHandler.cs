using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.GetUserExpProgres.Exceptions;
using UserManagementService.Application.V1.GetUserExpProgres.Repository;
using UserManagementService.Application.V1.GetUserExpProgres.Util;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.GetUserExpProgres;

public record GetUserExpProgressRequest(string userId) : IRequest<Progress>;

public class GetUserExpProgressHandler: IRequestHandler<GetUserExpProgressRequest, Progress>
{
    private readonly ILogger<GetUserExpProgressHandler> _logger;
    private readonly IProgressRepository _progressRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;

    public GetUserExpProgressHandler(ILogger<GetUserExpProgressHandler> logger, IProgressRepository progressRepository, IUserRepository userRepository, ILevelRepository levelRepository)
    {
        _logger = logger;
        _progressRepository = progressRepository;
        _userRepository = userRepository;
        _levelRepository = levelRepository;
    }

    public async Task<Progress> Handle(GetUserExpProgressRequest request, CancellationToken cancellationToken)
    {
        if (!await _userRepository.UserExistsAsync(request.userId))
        {
            throw new UserNotFoundException(request.userId);
        }

        var progress = await _progressRepository.GetUserExpProgressAsync(request.userId);

        var allLevels = await _levelRepository.GetAllLevelsAsync();
        var level = ResolveLevel(allLevels, progress.TotalExp);
        if (level is null)
        {
            throw new UnableToResolveLevelException(progress.TotalExp);
        }
        progress.Level = level;
        progress.Stage = ResolveStage(progress.Level);
        
        return progress;
    }

    private Level? ResolveLevel(IEnumerable<Level> allLevels, long totalExp)
    {
        return allLevels.First(level => totalExp.IsBetween(level.MinExp, level.MaxExp));
    }

    private int ResolveStage(Level level)
    {
        return level.Value switch
        {
            >= 1 and <= 5 => 1,
            > 5 and <= 10 => 2,
            > 10 and <= 15 => 3,
            > 15 => 4,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}