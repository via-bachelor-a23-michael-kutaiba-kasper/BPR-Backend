namespace RecommendationService.Application.V1.GetRecommendations.Engine;

public static class Scaling
{
    public static IEnumerable<float> ScaleToRange(float lowerBound, float upperBound, IEnumerable<float> data)
    {
        float Scale(float min, float max, float dataPoint)
        {
            return min + ((dataPoint - min) / (max - min)) * (max - min);
        }

        var enumerable = data as float[] ?? data.ToArray();
        
        float max = enumerable.Max();
        float min = enumerable.Min();

        return enumerable.Select(x => Scale(min, max, x));
    }
}