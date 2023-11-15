using System.ComponentModel;

namespace EventManagementService.Infrastructure.Util;

public static class EnumExtensions
{
    public static string GetDescription<T>(this T value) where T : Enum
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attributes = (DescriptionAttribute[])fieldInfo!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
}