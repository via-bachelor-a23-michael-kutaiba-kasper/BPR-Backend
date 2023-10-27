using System.Text.Json;
using EventManagementService.Domain.Models.Google;
using EventManagementService.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace EventManagementService.Application.FetchAllPublicEvents.Repository;

public interface IGeoCoding
{
    Task<GoogleGeoLocation> FetchGeoLocationForAddress(string address);
}

public class GeoCoding : IGeoCoding
{
    private readonly ILogger<GeoCoding> _logger;
    private readonly HttpClient _httpClient;

    public GeoCoding(ILogger<GeoCoding> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("HTTP_CLIENT");
    }

    public async Task<GoogleGeoLocation> FetchGeoLocationForAddress(string address)
    {
        _logger.LogInformation("Fetching the GeoLocation for address{Address}", address);
        var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        var uri = $"https://maps.googleapis.com/maps/api/geocode/json?address={address}&key={apiKey}";
        
        _logger.LogInformation($"API_KEY ->: {apiKey}");
        

        var result = await _httpClient.GetAsync(uri);

        _logger.LogInformation($"GEO RESULT -> {result}");
        ValidateHttpResponse(result);
        var responseString = await result.Content.ReadAsStringAsync();
        ValidateResponseContent(responseString);

        var response = JsonSerializer.Deserialize<GoogleGeoLocation>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        _logger.LogInformation($"GEO COUNT -> {response.Results.Count()}");
        return response!;
    }

    private static void ValidateHttpResponse(
        HttpResponseMessage responseMessage)
    {
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new HttpException
            (
                $"Unsuccessful response, code: {responseMessage}"
            );
        }
    }

    private static void ValidateResponseContent(string content)
    {
        if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
        {
            throw new HttpResponseException("Response is either null or empty");
        }
    }
}