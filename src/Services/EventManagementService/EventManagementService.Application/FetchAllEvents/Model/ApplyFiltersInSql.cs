namespace EventManagementService.Application.FetchAllEvents.Model;

public class ApplyFiltersInSql: IApplyFiltersStrategy
{
    public object Apply(Filters filters)
    {
        
        List<string> queryStrings = new();
        if (filters.From == null && filters.To == null && filters.HostId == null)
        {
            return "";
        }

        if (filters.From != null)
        {
            queryStrings.Add($"end_date <= @from");
        }
        
        if (filters.To != null)
        {
            queryStrings.Add($"start_date <= @to");
        }

        if (filters.HostId != null)
        {
            queryStrings.Add($"host_id=@hostId");
        }

        return string.Join(" AND ", queryStrings);
    }
}