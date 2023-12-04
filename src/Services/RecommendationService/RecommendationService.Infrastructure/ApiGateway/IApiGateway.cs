namespace RecommendationService.Infrastructure.ApiGateway;

public interface IApiGateway
{
    public Task<GatewayResponse<T>> QueryAsync<T>();
}