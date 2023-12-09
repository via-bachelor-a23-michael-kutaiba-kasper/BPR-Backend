using Microsoft.Extensions.Logging;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

/// <summary>
/// Handles registering exp to user for a exp generating event. 
/// </summary>
public interface IExpStrategy
{
    public Task Register(ExperienceGainedLedger ledger, ILogger logger);
}