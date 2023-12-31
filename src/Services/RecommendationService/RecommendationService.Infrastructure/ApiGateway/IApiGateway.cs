using System.Text;
using Google.Apis.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RecommendationService.Infrastructure.AppSettings;
using StringContent = System.Net.Http.StringContent;

namespace RecommendationService.Infrastructure.ApiGateway;

public interface IApiGateway
{
    public Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query, string queryName);
}

public class ApiGateway : IApiGateway
{
    private readonly HttpClient _client;
    private readonly Gateway _gatewayConfig;

    public ApiGateway(Gateway gatewayConfig, HttpClient? client = null)
    {
        _client = client ?? new HttpClient();
        _client.DefaultRequestHeaders.Add("Accept", "application/json");

        _gatewayConfig = gatewayConfig;
    }

    public async Task<GatewayResponse<T>> QueryAsync<T>(ApiGatewayQuery query, string queryName)
    {
        string serializedQuery = Serialize(query);
        StringContent payload = new StringContent(serializedQuery, Encoding.UTF8, "application/json");

        HttpResponseMessage responseFromGateway = await _client.PostAsync(_gatewayConfig.Url, payload);
        string responseBodyJsonString = await responseFromGateway.Content.ReadAsStringAsync();

        dynamic jsonElement = Deserialize<JObject>(responseBodyJsonString);
        JObject data = jsonElement.data[queryName];

        return data.ToObject<GatewayResponse<T>>();
    }

    private string Serialize<T>(T entity)
    {
        // NOTE: Using Newtonsoft instead of System.Text.Json since it can handle generics
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        return JsonConvert.SerializeObject(entity, new JsonSerializerSettings()
        {
            ContractResolver = contractResolver
        });
    }

    private T Deserialize<T>(string serializedEntity)
    {
        // NOTE: Using Newtonsoft instead of System.Text.Json since it can handle generics
        DefaultContractResolver contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        return JsonConvert.DeserializeObject<T>(serializedEntity, new JsonSerializerSettings()
        {
            ContractResolver = contractResolver
        });
    }
}