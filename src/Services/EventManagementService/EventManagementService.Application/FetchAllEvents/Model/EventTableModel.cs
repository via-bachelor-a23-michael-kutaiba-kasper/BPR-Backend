namespace EventManagementService.Application.FetchAllEvents.Model;

public class EventTableModel
{
    public string title { get; set; }
    public DateTimeOffset start_date { get; set; }
    public DateTimeOffset end_date { get; set; }
    public DateTimeOffset created_date { get; set; }
    public DateTimeOffset last_update_date { get; set; }
    public bool is_private { get; set; }
    public bool adult_only { get; set; }
    public bool is_free { get; set; }
    public string host_id { get; set; }
    public int max_number_of_attendees { get; set; }
    public string url { get; set; }
    public string description { get; set; }
    public string location { get; set; }
}

public class LocationTable
{
    public string street_number { get; set; }
    public string street_name { get; set; }
    public string sub_premise { get; set; }
    public string city { get; set; }
    public string postal_code { get; set; }
    public string country { get; set; }
    public float geolocation_lat { get; set; }
    public float geolocation_lng { get; set; }
}

public class Category
{
    public string name { get; set; }
}

public class Image
{
    public string uri { get; set; }
}

public class Keyword
{
    public string name { get; set; }
}