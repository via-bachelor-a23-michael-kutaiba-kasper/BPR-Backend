namespace EventManagementService.Application.V1.JoinEvent.Exceptions;

public class UserIsAlreadyHostOfEventException: Exception
{
    public UserIsAlreadyHostOfEventException(string userId, int eventId): base($"User {userId} is unable to join event {eventId} due to already being host of the event.")
    {
    }
}