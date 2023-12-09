namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IProgressRepository
{
    public Task AddExpToUserProgressAsync(string userId, long exp);
    public Task RegisterNewEventsHostedCount(string userId, int newEventsCount);
    public Task RegisterNewReviewCount(string userId, int newReviewCount);
}