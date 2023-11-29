using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using UserIdentifier = FirebaseAdmin.Auth.UserIdentifier;

namespace EventManagementService.Application.FetchAllEvents.Repository;

public interface IUserRepository
{
    public Task<IReadOnlyCollection<User>> GetUsersAsync(IReadOnlyCollection<string> userIds);
}

public class UserRepository : IUserRepository
{
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