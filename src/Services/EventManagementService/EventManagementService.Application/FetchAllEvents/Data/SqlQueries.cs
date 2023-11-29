namespace EventManagementService.Application.FetchAllEvents.Data;

public static class SqlQueries
{
    public static string QueryEventAttendees =>
        "SELECT user_id FROM public.event_attendee ea WHERE ea.event_id = @eventId";

    public static string AddAttendeeToEvent =>
        "INSERT INTO public.event_attendee(user_id, event_id) VALUES (@userId, @eventId)";

    public static string QueryAllFromEventTable => """
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
}