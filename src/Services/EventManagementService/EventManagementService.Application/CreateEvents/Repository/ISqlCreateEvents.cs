namespace EventManagementService.Application.CreateEvents.Repository;

public class ISqlCreateEvents
{
    
    /*public async Task UpsertEvents(IReadOnlyCollection<Event> events)
    {
        _logger.LogInformation("Upserting public events");
        try
        {
            var command = InsertEventSql();

            using (var connection = new NpgsqlConnection(_options.Value.Postgres))
            {
                await connection.OpenAsync();
                foreach (var item in events)
                {
                    var parameters = new
                    {
                        @title = item.Title,
                        @url = item.Url,
                        @description = item.Description,
                        @location = JsonSerializer.Serialize(item.Location)
                    };
                    _logger.LogInformation($"Location ->: {JsonSerializer.Serialize(item.Location)}");
                    connection.Execute(command, parameters);
                }
            }
        }
        catch (Exception e)
        {
            throw new UpsertScraperEventsException($"Cannot insert or update scraper events: {e.Message}", e);
        }
    }*/
    
    private static string InsertEventSql()
    {
        //TODO: update this insert -> look into temp tables to insert and then use merge to copy data using binary copy
        //TODO: add new migration for access code
        return """
               INSERT INTO public.event(title,url,location,description)
               values (@title, @url, @location, @description)
               """;
    }
}