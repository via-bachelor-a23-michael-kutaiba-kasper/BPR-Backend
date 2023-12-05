using EventManagementService.Domain.Models.Events;

namespace RecommendationService.Application.V1.GetRecommendations.Engine;

public record FrequencyMaps(IDictionary<Category, int> CategoryFrequencyMap, IDictionary<Keyword, int> KeywordFrequencyMap);

public record WeightMaps(IDictionary<Category, float> CategoryWeights, IDictionary<Keyword, float> KeywordWeights);
