using System.Text.Json.Serialization;

namespace UserManagementService.Infrastructure.ApiGateway;

public class ResponseStatus
{
    public int Code { get; set; } = default;
    public string Message { get; set; } = "";
}

public class GatewayResponse<T>
{
    [property: JsonPropertyName(("status"))]
    public ResponseStatus Status { get; set; } = default!;
    [property: JsonPropertyName("result")]
    public T Result { get; set; } 
}