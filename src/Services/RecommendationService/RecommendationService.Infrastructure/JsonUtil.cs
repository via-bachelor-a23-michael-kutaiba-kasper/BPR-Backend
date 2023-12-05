using System.Text.Json;

namespace RecommendationService.Infrastructure;

public static class JsonUtil
{
    public static JsonElement GetJsonElement(this JsonElement jsonElement, string path)
    {
        if (jsonElement.ValueKind == JsonValueKind.Null ||
            jsonElement.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

        string[] segments =
            path.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

        for (int n = 0; n < segments.Length; n++)
        {
            jsonElement = jsonElement.TryGetProperty(segments[n], out JsonElement value) ? value : default;

            if (jsonElement.ValueKind == JsonValueKind.Null ||
                jsonElement.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }
        }

        return jsonElement;
    }
}