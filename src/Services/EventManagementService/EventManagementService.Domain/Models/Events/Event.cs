namespace EventManagementService.Domain.Models.Events;

public class Event
{
    public string Title { get; set; }
    public Location Location { get; set; }
    public string Url{ get; set; }
    public string? Description { get; set; }
}