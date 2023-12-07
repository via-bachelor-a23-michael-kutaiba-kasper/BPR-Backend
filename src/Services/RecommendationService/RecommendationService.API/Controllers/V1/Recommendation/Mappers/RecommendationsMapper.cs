using RecommendationService.API.Controllers.V1.Recommendation.Dtos;
using RecommendationService.Domain;

namespace RecommendationService.API.Controllers.V1.Recommendation.Mappers;

internal static class RecommendationsMapper
{
    internal static ReadRecommendationsDto FromDomainToRecommendationsDto(Recommendations domainEnitty)
    {
        return new ReadRecommendationsDto
        {
            Result = domainEnitty.Result.Select(FromDomainToRecommendationDto).ToList(),
            User = new ReadUserDto()
            {
                UserId = domainEnitty.User.UserId,
                DisplayName = domainEnitty.User.DisplayName,
                PhotoUrl = domainEnitty.User.PhotoUrl,
                LastSeenOnline = domainEnitty.User.LastSeenOnline,
                CreationDate = domainEnitty.User.CreationDate
            },
            EventsProcessed = domainEnitty.EventsProcessed.Select(EventMapper.FromEventToDto).ToList(),
            ReviewsProcessed = domainEnitty.ReviewsProcessed.Select(review => new ReadReviewDto()
            {
                Id = review.Id,
                Rate = review.Rate,
                EventId = review.EventId,
                ReviewDate = review.ReviewDate,
                ReviewerId = review.ReviewerId
            }).ToList()
        };
    }
    
    internal static ReadRecommendationDto FromDomainToRecommendationDto(Domain.Recommendation domainEntity)
    {
        return new ReadRecommendationDto
        {
            Event = EventMapper.FromEventToDto(domainEntity.Event),
            RelevanceScore = domainEntity.RelevanceScore
        };
    }
}