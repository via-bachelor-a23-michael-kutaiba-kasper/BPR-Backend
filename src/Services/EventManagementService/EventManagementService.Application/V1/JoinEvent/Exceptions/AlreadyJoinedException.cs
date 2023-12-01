namespace EventManagementService.Application.V1.JoinEvent.Exceptions;

public class AlreadyJoinedException: Exception
{
    public AlreadyJoinedException(string userId, int eventId, Exception? inner = null): base($"User with id {userId} has already joined event with id{eventId}", inner)
    {
    }
}