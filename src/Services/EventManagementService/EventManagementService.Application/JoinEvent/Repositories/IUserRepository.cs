namespace EventManagementService.Application.JoinEvent.Repositories;

public interface IUserRepository
{
    Task<bool> UserExists(string userId);
}