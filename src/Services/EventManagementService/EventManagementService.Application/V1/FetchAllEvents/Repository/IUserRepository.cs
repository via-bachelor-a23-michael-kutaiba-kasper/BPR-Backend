using EventManagementService.Application.V1.FetchAllEvents.Exceptions;
using EventManagementService.Domain.Models;
using EventManagementService.Infrastructure;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;
using UserIdentifier = FirebaseAdmin.Auth.UserIdentifier;

namespace EventManagementService.Application.V1.FetchAllEvents.Repository;

public interface IUserRepository
{
    public Task<IReadOnlyCollection<User>> GetUsersAsync(IReadOnlyCollection<string> userIds);
}

public class UserRepository : IUserRepository
{

    private readonly ILogger<UserRepository> _logger;
        
    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
    }
    
    public async Task<IReadOnlyCollection<User>> GetUsersAsync(IReadOnlyCollection<string> userIds)
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
            throw new FailedToFetchUsersException(userIds, e);
        }
    }
}