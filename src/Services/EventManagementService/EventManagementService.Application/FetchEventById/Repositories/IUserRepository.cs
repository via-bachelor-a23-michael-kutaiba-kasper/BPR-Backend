using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;

namespace EventManagementService.Application.FetchEventById.Repositories;

public interface IUserRepository
{
    public Task<User?> GetUserById(string userId);
}

public class UserRepository : IUserRepository
{
    public async Task<User?> GetUserById(string userId)
    {
        var auth = FirebaseAuth.DefaultInstance;
        if (auth is null)
        {
            Firestore.CreateFirebaseApp();
            auth = FirebaseAuth.DefaultInstance;
        }

        try
        {
            var userRecord = await auth!.GetUserAsync(userId);
            
            return new User
            {
                UserId = userRecord.Uid,
                DisplayName = userRecord.DisplayName,
                PhotoUrl = userRecord.PhotoUrl,
                CreationDate = userRecord.UserMetaData.CreationTimestamp!.Value,
                LastSeenOnline = userRecord.UserMetaData.LastSignInTimestamp
            };
        }
        catch (FirebaseAuthException)
        {
            return null;
        }
    }
}