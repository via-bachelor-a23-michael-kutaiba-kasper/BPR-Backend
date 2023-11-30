namespace EventManagementService.Application.FetchAllEvents.Model;

public interface IApplyFiltersStrategy
{
    public object Apply(Filters filters);
}