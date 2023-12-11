using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.FetchUserAchievements.Repository;

public interface IUserRepository
{
    Task<bool> UserExists(string userId);
}

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }

    public async Task<bool> UserExists(string userId)
    {
        var defaultInstance = FirebaseAuth.DefaultInstance;
        if (defaultInstance == null)
        {
            Firestore.CreateFirebaseApp();
        }

        try
        {
            await FirebaseAuth.DefaultInstance.GetUserAsync(userId);
            return true;
        }
        catch (FirebaseAuthException e)
        {
            _logger.LogInformation(e.Message);
            return false;
        }
    }
}