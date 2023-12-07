namespace RecommendationService.Application.V1.GetRecommendations.Engine;

public static class Scaling
{
    public static IEnumerable<float> ScaleToRange(float lowerBound, float upperBound, IEnumerable<float> data)
    {
        float Scale(float min, float max, float dataPoint)
        {
            if (Math.Abs(max - min) < 0.001)
            {
                return min;
            }

            var scaledValue = ((dataPoint - min) / (max - min)) * (max - min) + max;
            return scaledValue;
        }

        var enumerable = data as float[] ?? data.ToArray();
        
        return enumerable.Select(x => Scale(lowerBound, upperBound, x));
    }
}