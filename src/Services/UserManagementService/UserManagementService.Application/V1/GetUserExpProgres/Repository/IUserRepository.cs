
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.GetUserExpProgres.Repository;

public interface IUserRepository
{
    public Task<bool> UserExistsAsync(string userId);
}

public class FirestoreUserRepository : IUserRepository
{
    private readonly ILogger<FirestoreUserRepository> _logger;

    public FirestoreUserRepository(ILogger<FirestoreUserRepository> logger)
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
            _logger.LogInformation(e.StackTrace);
            return false;
        }
    }
}