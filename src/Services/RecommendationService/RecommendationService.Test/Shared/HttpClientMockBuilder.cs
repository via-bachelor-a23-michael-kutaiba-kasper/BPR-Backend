using System.Net;
using Moq;
using Moq.Protected;

namespace RecommendationService.Test.Shared;

public class HttpClientMockBuilder
{
    private Dictionary<string, HttpResponseMessage> _responseMap = new();
    private Mock<HttpMessageHandler> _messageHandlerMock = new();
    private HttpClient _client;

    public HttpClientMockBuilder()
    {
        _client = new(_messageHandlerMock.Object);
    }

    public HttpClientMockBuilder RegisterGetEndpoint(string endpoint, HttpStatusCode code, string response)
    {
        HttpResponseMessage mockedResponse = new()
        {
            Content = new StringContent(response),
            StatusCode = code
        };


        Console.WriteLine(endpoint);
        _responseMap[new Uri(endpoint).AbsoluteUri] = mockedResponse;

        _messageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
            {
                Console.WriteLine(request.RequestUri.AbsoluteUri);
                if (_responseMap.TryGetValue(request.RequestUri.AbsoluteUri, out HttpResponseMessage response))
                {
                    return response;
                }

                throw new InvalidOperationException();
            });

        return this;
    }

    public HttpClientMockBuilder SetBaseUri(Uri uri)
    {
        _client.BaseAddress = uri;
        return this;
    }

    public HttpClient Build() => _client;
}