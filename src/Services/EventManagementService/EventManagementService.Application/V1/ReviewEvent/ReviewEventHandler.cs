using EventManagementService.Application.V1.ReviewEvent.Exceptions;
using EventManagementService.Application.V1.ReviewEvent.Repository;
using EventManagementService.Domain.Models.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EventManagementService.Application.V1.ReviewEvent;

public record ReviewEventRequest(Review Review) : IRequest<Review>;

public class ReviewEventHandler : IRequestHandler<ReviewEventRequest, Review>
{
    private readonly ISqlReviewEvent _sqlReviewEvent;
    private readonly IPubSubReviewEvent _pubSubReviewEvent;
    private readonly ILogger<ReviewEventHandler> _logger;

    public ReviewEventHandler
    (
        ISqlReviewEvent sqlReviewEvent,
        IPubSubReviewEvent pubSubReviewEvent,
        ILogger<ReviewEventHandler> logger
    )
    {
        _sqlReviewEvent = sqlReviewEvent;
        _pubSubReviewEvent = pubSubReviewEvent;
        _logger = logger;
    }

    public async Task<Review> Handle
    (
        ReviewEventRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var alreadyReviewed = await _sqlReviewEvent.UserAlreadyMadeReview(request.Review);
            if (alreadyReviewed)
            {
                throw new ReviewAlreadyExistException($"User cannot review same event more than once");
            }

            var reviewId = await _sqlReviewEvent.CreateEventReview(request.Review);
            var newReview = new Review
            {
                Id = reviewId,
                EventId = request.Review.EventId,
                ReviewerId = request.Review.ReviewerId,
                ReviewDate = request.Review.ReviewDate,
                Rate = request.Review.Rate
            };

            await _pubSubReviewEvent.PublishReviewedEvent(newReview);

            _logger.LogInformation($"Review has been successfully created at: {DateTimeOffset.UtcNow}");
            return newReview;
        }
        catch (Exception e)
        {
            _logger.LogError($"Review");
            throw new CreateEventReviewException("Review cannot be created", e);
        }
    }
}