using Newtonsoft.Json;

namespace RecommendationService.Infrastructure.ApiGateway;

public class ResponseStatus
{
    public int Code { get; set; } = default;
    public string Message { get; set; } = "";
}

public class GatewayResponse<T>
{
    [JsonProperty("status")]
    public ResponseStatus Status { get; set; } = default!;
    [JsonProperty("result")]
    public T Result { get; set; } 
}