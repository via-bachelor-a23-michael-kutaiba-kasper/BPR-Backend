using UserManagementService.Domain.Models;
using UserManagementService.Domain.Util;

namespace UserManagementService.Infrastructure.Util;

public static class CategoryExtensions
{
    public static string GetCategoryGroup(this Category category)
    {
        var field = category.GetType().GetField(category.ToString());
        var attribute = (CategoryGroupAttribute)Attribute.GetCustomAttribute(field, typeof(CategoryGroupAttribute));

        return attribute?.Group ?? "Uncategorized";
    }
}