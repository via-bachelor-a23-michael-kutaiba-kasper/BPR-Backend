using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.ProcessExpProgress.Repository;

public interface IReviewRepository
{
    public Task<IReadOnlyCollection<Review>> GetNewReviews();
    public Task<int> GetReviewsCountByUser(string userId);
}