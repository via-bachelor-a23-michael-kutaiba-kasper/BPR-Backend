using EventManagementService.Domain.Models;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.V1.GetRecommendations.Exceptions;
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
            _logger.LogWarning("Failed to fetch user from firebase, user does potentially not exist.");
            _logger.LogWarning(e.Message);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to fetch user from firebase");
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            throw new FailedToFetchException($"{nameof(User)}", "Firebase", userId, e);
        }
    }
}