using EventManagementService.API.Controllers.V1.ReviewControllers.Dtos;
using EventManagementService.Application.V1.FetchReviewsByUser.Model;
using EventManagementService.Domain.Models.Events;

namespace EventManagementService.API.Controllers.V1.ReviewControllers.Mappers;

internal static class ReviewMapper
{
    internal static Review ProcessIncomingReview(ReviewDto reviewDto)
    {
        return new Review
        {
            ReviewerId = reviewDto.ReviewerId,
            ReviewDate = reviewDto.ReviewDate,
            Rate = reviewDto.Rate,
            EventId = reviewDto.EventId
        };
    }

    internal static ReviewDto FromReviewToDto(Review review)
    {
        return new ReviewDto
        {
            EventId = review.EventId,
            ReviewerId = review.ReviewerId,
            ReviewDate = review.ReviewDate,
            Rate = review.Rate,
            Id = review.Id
        };
    }

    internal static IReadOnlyCollection<EventReviewDto> FromEventReviewToDto
    (
        IReadOnlyCollection<EventReview> eventReviews
    )
    {
        return eventReviews.Select(er => new EventReviewDto
            {
                ReviewerId = er.UserId,
                Rate = er.Rate,
                EventId = er.EventId,
                ReviewDate = er.ReviewDate,
                Id = er.ReviewId
            })
            .ToList();
    }
}