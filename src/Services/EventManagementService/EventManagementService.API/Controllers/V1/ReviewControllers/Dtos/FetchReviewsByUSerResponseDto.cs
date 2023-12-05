namespace EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;

public class FetchReviewsByUSerResponseDto
{
    public EventReviewDto[] Result { get; set; }
    public StatusCode status { get; set; }
}