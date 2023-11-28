using System.Text.Json.Serialization;

namespace EventManagementService.API.Controllers.V1.EventControllers.Dtos;

public class UserDto
{
    [JsonPropertyName("userId")]
    public String UserId{ get; set; }
    
    [JsonPropertyName("photoUrl")]
    public String? PhotoUrl{ get; set; }
    
    [JsonPropertyName("displayName")]
    public String DisplayName{ get; set; }
    
    [JsonPropertyName("dateOfBirth")]
    public DateTimeOffset DateOfBirth{ get; set; }
    
    [JsonPropertyName("lastSeenOnline")]
    public DateTimeOffset? LastSeenOnline{ get; set; }
    
    [JsonPropertyName("creationDate")]
    public DateTimeOffset CreationDate{ get; set; }
}