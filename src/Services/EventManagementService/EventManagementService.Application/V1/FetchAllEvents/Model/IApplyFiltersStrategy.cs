namespace EventManagementService.Application.V1.FetchAllEvents.Model;

public interface IApplyFiltersStrategy
{
    public object Apply(Filters filters);
}