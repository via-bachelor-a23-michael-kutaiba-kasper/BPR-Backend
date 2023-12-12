using System.Text.Json;
using EventManagementService.Domain.Models;
using EventManagementService.Domain.Models.Events;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Logging;
using RecommendationService.Domain;
using RecommendationService.Domain.Events;
using RecommendationService.Domain.Util;
using RecommendationService.Infrastructure;

namespace RecommendationService.Application.V1.GetInterestSurvey.Repositories;

public interface IInterestSurveyRepository
{
    public Task<InterestSurvey?> GetInterestSurvey(string userId);
}

public class FirebaseInterestSurveyRepository : IInterestSurveyRepository
{
    private readonly CollectionReference _reference;
    private readonly ILogger<FirebaseInterestSurveyRepository> _logger;

    public FirebaseInterestSurveyRepository(ILogger<FirebaseInterestSurveyRepository> logger)
    {
        _logger = logger;
        _reference = Firestore.Get().Collection("interestSurveys");
    }

    public async Task<InterestSurvey?> GetInterestSurvey(string userId)
    {
        var docRef = _reference.Document(userId);
        if (docRef is null)
        {
            return null;
        }

        var snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return null;
        }

        var data = snapshot.ToDictionary();
        if (!data.ContainsKey("interestSurvey"))
        {
            return null;
        }

        var surveySerialized = (string)data["interestSurvey"];
        if (surveySerialized is null)
        {
            return null;
        }


        var surveyDto = JsonSerializer.Deserialize<InterestSurveyDto>(surveySerialized, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        if (surveyDto is null)
        {
            return null;
        }

        return new InterestSurvey
        {
            User = surveyDto.User,
            Categories = surveyDto.Categories.Select(EnumExtensions.GetEnumValueFromDescription<Category>).ToList(),
            Keywords = surveyDto.Keywords.Select(EnumExtensions.GetEnumValueFromDescription<Keyword>).ToList()
        };
    }

    private class InterestSurveyDto
    {
        public User User { get; set; }
        public IReadOnlyCollection<string> Keywords { get; set; }
        public IReadOnlyCollection<string> Categories { get; set; }
    }
}