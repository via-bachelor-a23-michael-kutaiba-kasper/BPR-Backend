using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.JoinEvent.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string userId);
}

public class UserRepository: IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }
    public async Task<bool> UserExistsAsync(string userId)
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