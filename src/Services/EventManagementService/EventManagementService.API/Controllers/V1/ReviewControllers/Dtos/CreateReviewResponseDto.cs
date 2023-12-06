using System.Net;

namespace EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;

public class CreateReviewResponseDto
{
    public ReviewDto Result { get; set; }
    public StatusCode status { get; set; }
}

public class StatusCode
{
    public HttpStatusCode Code { get; set; }
    public string Message { get; set; }
}