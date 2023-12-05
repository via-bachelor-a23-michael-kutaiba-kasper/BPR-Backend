namespace EventManagementService.Application.V1.FetchAllEvents.Model;

public class ApplyFiltersInSql: IApplyFiltersStrategy
{
    public object Apply(Filters filters)
    {
        
        List<string> queryStrings = new();

        if (filters.From != null)
        {
            queryStrings.Add($"end_date >= @from");
        }
        
        if (filters.To != null)
        {
            queryStrings.Add($"start_date <= @to");
        }

        if (filters.HostId != null)
        {
            queryStrings.Add($"host_id=@hostId");
        }


        if (!filters.IncludePrivateEvents)
        {
            queryStrings.Add($"is_private=false");
        }
        else
        {
            queryStrings.Add("(is_private=false or is_private=true)");
        }

        return $"WHERE {string.Join(" AND ", queryStrings)}";
    }
}