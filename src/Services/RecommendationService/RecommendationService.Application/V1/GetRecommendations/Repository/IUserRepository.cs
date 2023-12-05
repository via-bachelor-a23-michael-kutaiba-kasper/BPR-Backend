using EventManagementService.Domain.Models;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using RecommendationService.Infrastructure;

namespace RecommendationService.Application.V1.GetRecommendations.Repository;

public interface IUserRepository
{
    Task<User?> GetById(string userId);
}

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public async Task<User?> GetById(string userId)
    {
        var defaultInstance = FirebaseAuth.DefaultInstance;
        if (defaultInstance == null)
        {
            Firestore.CreateFirebaseApp();
        }

        try
        {
            var record = await FirebaseAuth.DefaultInstance.GetUserAsync(userId);
            return new User
            {
                UserId = record.Uid,
                CreationDate = record.UserMetaData.CreationTimestamp.Value,
                DisplayName = record.DisplayName,
                PhotoUrl = record.PhotoUrl,
                LastSeenOnline = record.UserMetaData.LastSignInTimestamp
            };
        }
        catch (FirebaseAuthException e)
        {
            _logger.LogInformation(e.Message);
            return null;
        }
    }
}