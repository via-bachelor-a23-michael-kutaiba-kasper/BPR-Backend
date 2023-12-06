namespace EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;

public class EventReviewDto
{
    public int ReviewId { get; set; }
    public float Rate { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
    public int EventId { get; set; }
}