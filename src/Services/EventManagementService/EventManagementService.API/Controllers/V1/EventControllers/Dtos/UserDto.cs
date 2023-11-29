using System.Text.Json.Serialization;

namespace EventManagementService.API.Controllers.V1.EventControllers.Dtos;

public class UserDto
{
    [JsonPropertyName("userId")]
    public string UserId{ get; set; }
    
    [JsonPropertyName("photoUrl")]
    public string? PhotoUrl{ get; set; }
    
    [JsonPropertyName("displayName")]
    public string DisplayName{ get; set; }
    
    [JsonPropertyName("lastSeenOnline")]
    public DateTimeOffset? LastSeenOnline{ get; set; }
    
    [JsonPropertyName("creationDate")]
    public DateTimeOffset CreationDate{ get; set; }
}