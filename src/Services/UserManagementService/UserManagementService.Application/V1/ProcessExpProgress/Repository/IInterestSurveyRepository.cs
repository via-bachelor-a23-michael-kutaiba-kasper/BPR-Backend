namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IInterestSurveyRepository
{
    Task<IReadOnlyCollection<string>> GetNewlyCreatedSurveyUserList();
}