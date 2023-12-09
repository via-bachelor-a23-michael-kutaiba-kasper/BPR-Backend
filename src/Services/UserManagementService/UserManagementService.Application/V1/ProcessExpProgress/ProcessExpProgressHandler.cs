using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public record ProcessExpProgressRequest() : IRequest;

public class ProcessExpProgressHandler : IRequestHandler<ProcessExpProgressRequest>
{
    private readonly ExperienceGainedLedger _ledger = new ExperienceGainedLedger();
    private readonly ILogger<ProcessExpProgressHandler> _logger;
    private readonly IEnumerable<IExpStrategy> _expStrategies;
    private readonly IProgressRepository _progressRepository;
    public ProcessExpProgressHandler(ILogger<ProcessExpProgressHandler> logger, IEnumerable<IExpStrategy> expStrategies, IProgressRepository progressRepository)
    {
        _logger = logger;
        _expStrategies = expStrategies;
        _progressRepository = progressRepository;
    }

    public async Task Handle(ProcessExpProgressRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing experience gains");
        foreach (var expStrategy in _expStrategies)
        {
            await expStrategy.Register(_ledger, _logger);
        }
        await CommitNewExperienceGains();
    }

    private async Task CommitNewExperienceGains()
    {
        var userIds = _ledger.GetUserIds();
        foreach (var userId in userIds)
        {
            await _progressRepository.AddExpToUserProgressAsync(userId, _ledger.GetExperienceGained(userId));
        }
    }
}