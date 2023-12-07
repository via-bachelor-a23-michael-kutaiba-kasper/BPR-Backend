namespace RecommendationService.Test.Shared;

public class HttpClientMock: HttpClient
{
    public override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.Send(request, cancellationToken);
    }
}