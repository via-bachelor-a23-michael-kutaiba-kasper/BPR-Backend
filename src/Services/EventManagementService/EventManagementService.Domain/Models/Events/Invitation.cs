namespace EventManagementService.Domain.Models.Events;

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined
}

public static class InvitationStatusExtensions
{
    public static string ToString(this InvitationStatus status)
    {
        return status switch
        {
            InvitationStatus.Accepted => "Accepted",
            InvitationStatus.Declined => "Declined",
            InvitationStatus.Pending => "Pending",
            _ => throw new Exception($"Invalid invitation status encountered")
        };
    }

    public static InvitationStatus FromString(this string s)
    {
        return s.ToLowerInvariant() switch
        {
            "accepted" => InvitationStatus.Accepted,
            "declined" => InvitationStatus.Declined,
            "pending" => InvitationStatus.Pending,
            _ => throw new Exception($"Cannot parse {s} into InvitationStatus")
        };
    }
}

public class Invitation
{
    public int Id { get; set; }
    public InvitationStatus Status { get; set; }
    public string UserId { get; set; }
    public int EventId{ get; set; }
}