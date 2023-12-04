namespace EventManagementService.Domain.Models.Events;

public class Review
{
    public int Id { get; set; }
    public float Rate { get; set; }
    public string ReviewerId { get; set; } = default!;
    public int EventId { get; set; }
    public DateTimeOffset ReviewDate { get; set; } = default!;
}