using System.Text.Json;
using EventManagementService.Domain.Models;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;

namespace RecommendationService.Application.V1.StoreInterestSurveyResult.Repositories;

public interface IInterestSurveyRepository
{
    public Task<InterestSurvey?> GetInterestSurvey(string userId);
    public Task<InterestSurvey> StoreInterestSurvey(string userId, InterestSurvey survey);
}

public class FirebaseInterestSurveyRepository : IInterestSurveyRepository
{
    private readonly CollectionReference _reference;
    private readonly ILogger<FirebaseInterestSurveyRepository> _logger;

    public FirebaseInterestSurveyRepository(ILogger<FirebaseInterestSurveyRepository> logger)
    {
        _logger = logger;
    }

    public Task<InterestSurvey?> GetInterestSurvey(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<InterestSurvey> StoreInterestSurvey(string userId, InterestSurvey survey)
    {
        var docRef = _reference.Document(userId);

        var surveyDto = new InterestSurveyDto()
        {
            User = survey.User,
            Categories = survey.Categories.Select(category => category.GetDescription()).ToList(),
            Keywords= survey.Keywords.Select(kw => kw.GetDescription()).ToList(),
            
        }
        var surveyDtoSerialized = JsonSerializer.Serialize(surveyDto, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var data = new Dictionary<string, string>
        {
            { "interestSurvey", surveyDtoSerialized }
        };
        await docRef.SetAsync(data);

        return survey;
    }

    class InterestSurveyDto
    {
        public User User { get; set; }
        public IReadOnlyCollection<string> Keywords{ get; set; }
        public IReadOnlyCollection<string> Categories{ get; set; }
    }
}