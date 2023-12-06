using Moq;

namespace RecommendationService.Test.Shared;

public class TestingUtil
{
    private const string HttpClientName = "HTTP_CLIENT";
    public static Mock<IHttpClientFactory> CreateHttpClientFactoryMock(Action<HttpClientMockBuilder> configureClient)
    {
        var mockBuilder = new HttpClientMockBuilder();
        configureClient(mockBuilder);
        
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(x => x.CreateClient(HttpClientName)).Returns(mockBuilder.Build());
        
        return mockFactory;
    }
}