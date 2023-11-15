namespace EventManagementService.Application.JoinEvent.Data;

public class SqlQueries
{
    public static string GetAttendeesOfEvent =>
        "SELECT user_id FROM public.event_attendee ea WHERE ea.event_id = @eventId";

    public static string AddAttendeeToEvent =>
        "INSERT INTO public.event_attendee(user_id, event_id) VALUES (@userId, @eventId)";

    public static string GetEventsByUser =>
        """
        SELECT
            e.*,
            l.*
        FROM
            postgres.public.event e
        JOIN
                location l ON e.location_id = l.id
        LEFT JOIN
                public.event_attendee ea on e.id = ea.event_id
        LEFT JOIN
                event_category ec ON e.id = ec.event_id
        LEFT JOIN
                category c ON ec.category_id = c.id
        LEFT JOIN
                keyword_category kc ON e.id = kc.event_id
        LEFT JOIN
                keyword k ON kc.keyword = k.id
        WHERE e.host_id = @hostId
        """;
    public static string GetEventById=>
        """
        SELECT
            e.*,
            l.*
        FROM
            postgres.public.event e
        JOIN
                location l ON e.location_id = l.id
        LEFT JOIN
                public.event_attendee ea on e.id = ea.event_id
        LEFT JOIN
                event_category ec ON e.id = ec.event_id
        LEFT JOIN
                category c ON ec.category_id = c.id
        LEFT JOIN
                keyword_category kc ON e.id = kc.event_id
        LEFT JOIN
                keyword k ON kc.keyword = k.id
        WHERE e.id = @eventId
        """;
}