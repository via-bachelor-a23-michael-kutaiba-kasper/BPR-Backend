
using System.Text.Json.Serialization;

namespace EventManagementService.API.Controllers.V1.Dtos;

public class JoinEventDto
{
    [JsonPropertyName("userId")] public string UserId { get; set; }
    [JsonPropertyName("eventId")] public int EventId { get; set; }
}
    
