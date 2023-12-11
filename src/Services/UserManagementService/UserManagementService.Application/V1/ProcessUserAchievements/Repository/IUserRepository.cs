using System.Text.Json;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using UserManagementService.Application.V1.ProcessUserAchievements.Exceptions;
using UserManagementService.Domain.Models;
using UserManagementService.Infrastructure;

namespace UserManagementService.Application.V1.ProcessUserAchievements.Repository;

public interface IUserRepository
{
    Task<bool> UserExists(string userId);
    Task<string> GetNotificationTokenByUserIdAsync(string userId);
}

public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly CollectionReference _collectionReference;

    public UserRepository(ILogger<UserRepository> logger)
    {
        _logger = logger;
        _collectionReference = Firestore.Get().Collection("notifications");
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

    public async Task<string> GetNotificationTokenByUserIdAsync(string userId)
    {
        var userTokenCollection = _collectionReference.Document(userId);
        var snapshot = await userTokenCollection.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            // TODO: Make custom exception
            throw new Exception("User device has not been registered with FCM");
        }
        
        var data = snapshot.ToDictionary();
        if (!data.ContainsKey("token"))
        {
            throw new Exception("User device has not been registered with FCM");
        }

        Console.WriteLine(JsonSerializer.Serialize(data));

        return (string) data["token"];
    }
}