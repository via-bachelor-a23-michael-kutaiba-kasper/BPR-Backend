namespace EventManagementService.Application.JoinEvent.Data;

public class SqlQueries
{
    public static string GetAttendeesOfEvent => "SELECT user_id FROM public.event_attendee ea WHERE ea.event_id = @eventId";

    public static string AddAttendeeToEvent =>
        "INSERT INTO public.event_attendee(user_id, event_id) VALUES (@userId, @eventId)";
}