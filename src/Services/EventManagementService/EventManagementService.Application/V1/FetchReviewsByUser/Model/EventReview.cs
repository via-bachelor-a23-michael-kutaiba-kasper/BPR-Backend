namespace EventManagementService.Application.V1.FetchReviewsByUser.Model;

public class EventReview
{
    public int ReviewId { get; set; }
    public float Rate { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
    public int EventId { get; set; }
}