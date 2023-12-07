namespace RecommendationService.Infrastructure.ApiGateway;

public class GqlResponse<T>
{
    public T Data { get; set; }
    public object[] Errors { get; set; }
}