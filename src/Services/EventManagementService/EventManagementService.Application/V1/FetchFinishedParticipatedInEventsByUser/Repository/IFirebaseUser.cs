using EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Repository;

public interface IFirebaseUser
{
    public Task<IReadOnlyCollection<User>> GetUsers(IReadOnlyCollection<string> userIds);
}

public class FirebaseUser : IFirebaseUser
{
    private readonly ILogger<FirebaseUser> _logger;

    public FirebaseUser(ILogger<FirebaseUser> logger)
    {
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<User>> GetUsers(IReadOnlyCollection<string> userIds)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError($"Failed to fetch users from firebase:\n{e.StackTrace}", e);
            throw new FailedToFetchUsersException($"Failed to fetch users ({string.Join(", ", userIds)})", e);
        }
    }
}