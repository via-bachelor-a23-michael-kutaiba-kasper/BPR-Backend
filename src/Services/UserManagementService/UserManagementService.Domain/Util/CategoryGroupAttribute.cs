namespace UserManagementService.Domain.Util;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class CategoryGroupAttribute : Attribute
{
    public string Group { get; }

    public CategoryGroupAttribute(string group)
    {
        Group = group;
    }
}