namespace EventManagementService.Application.V1.FetchFinishedParticipatedInEventsByUser.Util;

public class SqlUtil
{
    public static string AsIntList(IReadOnlyCollection<int> xs)
    {
        return xs.Count == 0 ? "" : $"({string.Join(", ", xs)})";
    }
}