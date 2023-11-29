namespace EventManagementService.Application.FetchAllEvents.Data;

public static class SqlUtil
{
    public static string AsIntList(IReadOnlyCollection<int> xs)
    {
        return $"({string.Join(", ", xs)})";
    }
}