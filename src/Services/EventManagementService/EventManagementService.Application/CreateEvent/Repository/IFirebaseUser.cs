using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.CreateEvent.Repository;

public interface IFirebaseUser
{
    Task<bool> UserExistsAsync(string userId);
}

public class FirebaseUser : IFirebaseUser
{
    private readonly ILogger<FirebaseUser> _logger;

    public FirebaseUser(ILogger<FirebaseUser> logger)
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