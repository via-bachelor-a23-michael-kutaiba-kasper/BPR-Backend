using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;

namespace EventManagementService.Application.V1.FetchEventById.Repositories;

public interface IUserRepository
{
    public Task<User?> GetUserById(string userId);
    public Task<IReadOnlyCollection<User>> GetUsersAsync(IReadOnlyCollection<string> userIds);
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

    public async Task<IReadOnlyCollection<User>> GetUsersAsync(IReadOnlyCollection<string> userIds)
    {
        var auth = FirebaseAuth.DefaultInstance;
        if (auth is null)
        {
            Firestore.CreateFirebaseApp();
            auth = FirebaseAuth.DefaultInstance;
        }

        List<Task<UserRecord>> userRecordTasks = new List<Task<UserRecord>>();
        foreach (var id in userIds)
        {
            userRecordTasks.Add(auth.GetUserAsync(id));
        }

        var userRecords = await Task.WhenAll(userRecordTasks);
        return userRecords.Select(u => new User()
        {
            CreationDate = u.UserMetaData.CreationTimestamp.Value,
            UserId = u.Uid,
            DisplayName = u.DisplayName,
            PhotoUrl = u.PhotoUrl,
            LastSeenOnline = u.UserMetaData.LastSignInTimestamp
        }).ToList();
    }
}