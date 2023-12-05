using EventManagementService.Domain.Models;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IUserRepository
{
    Task<User?> GetById(string userId);
}