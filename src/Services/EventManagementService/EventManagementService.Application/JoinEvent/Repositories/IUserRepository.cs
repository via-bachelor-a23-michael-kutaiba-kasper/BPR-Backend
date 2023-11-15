using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;

namespace EventManagementService.Application.JoinEvent.Repositories;

public interface IUserRepository
{
    Task<bool> UserExistsAsync(string userId);
}

public class UserRepository: IUserRepository
{
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
            return false;
        }
    }
}