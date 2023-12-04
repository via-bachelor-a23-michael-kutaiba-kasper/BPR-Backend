namespace RecommendationService.Infrastructure.ApiGateway;

public class ResponseStatus
{
    public int Code { get; set; } = default;
    public string Message { get; set; } = "";
}

public class GatewayResponse<T>
{
    public Type Status { get; set; } = default!;
    public T Result { get; set; }
}