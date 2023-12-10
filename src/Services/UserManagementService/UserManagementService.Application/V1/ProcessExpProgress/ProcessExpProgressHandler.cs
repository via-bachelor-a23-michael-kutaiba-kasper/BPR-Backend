using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public record ProcessExpProgressRequest(IEnumerable<IExpStrategy> strategies) : IRequest;

public class ProcessExpProgressHandler : IRequestHandler<ProcessExpProgressRequest>
{
    private readonly ExperienceGainedLedger _ledger;
    private readonly ILogger<ProcessExpProgressHandler> _logger;
    private readonly IProgressRepository _progressRepository;

    public ProcessExpProgressHandler(ILogger<ProcessExpProgressHandler> logger, IProgressRepository progressRepository, ExperienceGainedLedger? ledger = null)
    {
        _logger = logger;
        _progressRepository = progressRepository;
        _ledger = ledger ?? new ExperienceGainedLedger();
    }

    public async Task Handle(ProcessExpProgressRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing experience gains");
        foreach (var expStrategy in request.strategies)
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
            await _progressRepository.EnsureUserHasProgress(userId);
            await _progressRepository.AddExpToUserProgressAsync(userId, _ledger.GetExperienceGained(userId));
        }
        _logger.LogInformation("Updated exp progress for all relevant users!");
    }
}