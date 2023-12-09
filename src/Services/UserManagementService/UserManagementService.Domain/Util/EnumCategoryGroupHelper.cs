namespace UserManagementService.Domain.Util;

public static class EnumCategoryGroupHelper
{
    public static bool AreEnumsInSameCategoryGroup(Enum value1, Enum value2)
    {
        var groupAttribute1 = GetCategoryGroupAttribute(value1);
        var groupAttribute2 = GetCategoryGroupAttribute(value2);

        if (groupAttribute1 != null && groupAttribute2 != null)
        {
            return groupAttribute1.Group == groupAttribute2.Group;
        }
        
        return false;
    }

    public static CategoryGroupAttribute? GetCategoryGroupAttribute(Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo == null) return null;
        var attributes = fieldInfo.GetCustomAttributes(typeof(CategoryGroupAttribute), false);
        if (attributes.Length > 0)
        {
            return (CategoryGroupAttribute)attributes[0];
        }

        return null;
    }
}