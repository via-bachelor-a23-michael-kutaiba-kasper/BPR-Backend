using Newtonsoft.Json;

namespace EventManagementService.API.Controllers.V1.Dtos;

public record JoinEventDto(
    [JsonProperty("userId")] string UserId,
    [JsonProperty("eventId")] int EventId
);