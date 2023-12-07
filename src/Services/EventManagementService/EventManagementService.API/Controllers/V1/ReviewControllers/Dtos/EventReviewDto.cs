namespace EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;

public class EventReviewDto
{
    public int Id { get; set; }
    public float Rate { get; set; }
    public string ReviewerId { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
    public int EventId { get; set; }
}