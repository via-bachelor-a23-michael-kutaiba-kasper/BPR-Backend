namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IProgressRepository
{
    public Task AddExpToUserProgressAsync(string userId, long exp);
    public Task RegisterNewEventsHostedCount(int newEventsCount);
    public Task RegisterNew(int newEventsCount);
}