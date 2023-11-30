namespace EventManagementService.Application.FetchAllEvents.Exceptions;

public class FailedToFetchUsersException: Exception
{
    public FailedToFetchUsersException(IReadOnlyCollection<string> userIds, Exception? inner = null) : base(
        $"Failed to fetch users ({string.Join(", ", userIds)})", inner)

    {
    }
}