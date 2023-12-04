
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace RecommendationService.Infrastructure.ApiGateway;

public interface IApiGateway
{
    public Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query);
}

public class ApiGateway : IApiGateway
{
    private readonly ILogger<ApiGateway> _logger;
    private readonly HttpClient _client;
    public ApiGateway(ILogger<ApiGateway> logger)
    {
        _logger = logger;
        
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client = client;
    }
    public async Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query)
    {
        string serializedQuery = Serialize(query);
        StringContent payload = new StringContent(serializedQuery, Encoding.UTF8, "application/json");
        
        throw new NotImplementedException();
    }

    private string Serialize<T>(T entity)
    {
        return JsonSerializer.Serialize(entity, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }
}