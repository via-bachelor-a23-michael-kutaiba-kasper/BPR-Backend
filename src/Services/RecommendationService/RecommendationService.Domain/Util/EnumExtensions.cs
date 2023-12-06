using System.ComponentModel;

namespace RecommendationService.Domain.Util;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
    
    public static T GetEnumValueFromDescription<T>(string description)
    {
        foreach (var field in typeof(T).GetFields())
        {
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                {
                    return (T)field.GetValue(null);
                }
            }
        }

        throw new ArgumentException($"No enum value with the description '{description}' found in {typeof(T).FullName}.");
    }
}