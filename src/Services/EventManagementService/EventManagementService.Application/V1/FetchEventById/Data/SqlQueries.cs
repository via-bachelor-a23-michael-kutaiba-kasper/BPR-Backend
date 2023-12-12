namespace EventManagementService.Application.V1.FetchEventById.Data;

public static class SqlQueries
{
    public static string QueryEventAttendees =>
        "SELECT user_id FROM public.event_attendee ea WHERE ea.event_id = @eventId";

    public static string QueryAllFromEventTableByEventId => """
                                                            SELECT e.id,
                                                            title,
                                                            start_date,
                                                            end_date,
                                                            created_date,
                                                            is_private,
                                                            adult_only,
                                                            is_paid,
                                                            host_id,
                                                            max_number_of_attendees,
                                                            last_update_date,
                                                            url,
                                                            description,
                                                            location,
                                                            access_code,
                                                            category_id,
                                                            city,
                                                            geolocation_lat,
                                                            geolocation_lng
                                                            FROM event e
                                                            WHERE e.id = @eventId;
                                                            """;

    public static string QueryEventKeywords => """
                                               SELECT
                                               keyword
                                               FROM event_keyword
                                               WHERE event_id = @eventId
                                               """;

    public static string QueryEventImages => """
                                                SELECT URI
                                                FROM image
                                                WHERE event_id = @eventId
                                             """;

    public static string QueryAllFromEventTableByHostId => """
                                                           SELECT id,
                                                           title,
                                                           start_date,
                                                           end_date,
                                                           created_date,
                                                           is_private,
                                                           adult_only,
                                                           is_paid,
                                                           host_id,
                                                           max_number_of_attendees,
                                                           last_update_date,
                                                           url,
                                                           description,
                                                           location_id,
                                                           access_code,
                                                           category_id
                                                           FROM event e
                                                           WHERE host_id = @host_id;
                                                           """;
}