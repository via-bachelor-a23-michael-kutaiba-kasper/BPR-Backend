using System.Net;

namespace EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;

public class CreateReviewResponseDto
{
    public ReviewDto Review { get; set; }
    public StatusCode Code { get; set; }
}

public class StatusCode
{
    public HttpStatusCode Code { get; set; }
    public string Message { get; set; }
}