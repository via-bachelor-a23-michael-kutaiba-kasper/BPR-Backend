using EventManagementService.Application.V1.FetchReviewsByUser.Exceptions;
using EventManagementService.Application.V1.FetchReviewsByUser.Model;
using EventManagementService.Application.V1.FetchReviewsByUser.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.FetchReviewsByUser;

public record FetchReviewsByUserRequest(string? UserId) : IRequest<IReadOnlyCollection<EventReview>>;

public class FetchReviewsByUSerHandler : IRequestHandler<FetchReviewsByUserRequest, IReadOnlyCollection<EventReview>>
{
    private readonly ISqlReviews _sqlReviews;
    private readonly ILogger<FetchReviewsByUSerHandler> _logger;

    public FetchReviewsByUSerHandler(ISqlReviews sqlReviews, ILogger<FetchReviewsByUSerHandler> logger)
    {
        _sqlReviews = sqlReviews;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<EventReview>> Handle
    (
        FetchReviewsByUserRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrWhiteSpace(request.UserId))
                throw new InvalidUserIdException("The user id must not be null");
            return await _sqlReviews.FetchReviewsByUserId(request.UserId);
        }
        catch (Exception e)
        {
            _logger.LogError($"Unable to fetch event reviews for user: {request.UserId}, {e.StackTrace}");
            throw new FetchReviewsByUserException
            (
                $"Something went wrong while fetching reviews for user with id : {request.UserId} at: {DateTimeOffset.UtcNow}",
                e
            );
        }
    }
}