namespace EventManagementService.Application.FetchEventById.Data;

public class SqlQueries
{
    public static string QueryEventAttendees =>
        "SELECT user_id FROM public.event_attendee ea WHERE ea.event_id = @eventId";

    public static string AddAttendeeToEvent =>
        "INSERT INTO public.event_attendee(user_id, event_id) VALUES (@userId, @eventId)";

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
                                                            location_id,
                                                            access_code,
                                                            category_id,
                                                            street_name,
                                                            street_number,
                                                            sub_premise,
                                                            city,
                                                            postal_code,
                                                            country,
                                                            geolocation_lat,
                                                            geolocation_lng
                                                            FROM event e join location l on e.location_id = l.id
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