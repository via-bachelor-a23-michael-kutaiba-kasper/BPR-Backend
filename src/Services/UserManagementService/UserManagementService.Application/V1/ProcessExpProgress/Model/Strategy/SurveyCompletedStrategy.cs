using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessExpProgress.Model.ExpGeneratingEvents;
using UserManagementService.Application.V1.ProcessExpProgress.Repository;

namespace UserManagementService.Application.V1.ProcessExpProgress.Model.Strategy;

public class SurveyCompletedStrategy: IExpStrategy
{
    private readonly IInterestSurveyRepository _surveyRepository;
    
    public SurveyCompletedStrategy(IInterestSurveyRepository surveyRepository)
    {
        _surveyRepository = surveyRepository;
    }
    
    public async Task Register(ExperienceGainedLedger ledger, ILogger logger)
    {
        logger.LogInformation("Processing newly completed surveys experience gains");
        var newUsersThatHasCompletedSurveys = await _surveyRepository.GetNewlyCreatedSurveyUserList();
        foreach (var userId in newUsersThatHasCompletedSurveys)
        {
            ledger.RegisterExpGeneratingEvent(userId, e => new SurveyCompletedEvent(e));
        }
    }
}