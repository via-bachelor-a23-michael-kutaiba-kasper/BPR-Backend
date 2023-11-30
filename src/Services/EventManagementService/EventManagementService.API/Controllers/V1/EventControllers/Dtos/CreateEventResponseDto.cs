using System.Net;

namespace EventManagementService.API.Controllers.V1.EventControllers.Dtos;

public class CreateEventResponseDto
{
    public EventDto EventDto { get; set; }
    public StatusCode StatusCode { get; set; }
}

public class StatusCode
{
    public HttpStatusCode Code { get; set; }
    public string Message { get; set; }
}