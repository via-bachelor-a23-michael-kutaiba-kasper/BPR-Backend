
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RecommendationService.Infrastructure.AppSettings;

namespace RecommendationService.Infrastructure.ApiGateway;

public interface IApiGateway
{
    public Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query);
}

public class ApiGateway : IApiGateway
{
    private readonly HttpClient _client;
    private readonly Gateway _gatewayConfig;
    public ApiGateway(Gateway gatewayConfig)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client = client;

        _gatewayConfig = gatewayConfig;
    }
    public async Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query)
    {
        string serializedQuery = Serialize(query);
        StringContent payload = new StringContent(serializedQuery, Encoding.UTF8, "application/json");

        HttpResponseMessage responseFromGateway = await _client.PostAsync(_gatewayConfig.Url, payload);
        string responseBodyJsonString = await responseFromGateway.Content.ReadAsStringAsync();
        var gqlResponse = Deserialize<GqlResponse<GatewayResponse<T>>>(responseBodyJsonString);

        return gqlResponse.Data;
    }

    private string Serialize<T>(T entity)
    {
        return JsonSerializer.Serialize(entity, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    private T Deserialize<T>(string serializedEntity)
    {
        return JsonSerializer.Deserialize<T>(serializedEntity, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}