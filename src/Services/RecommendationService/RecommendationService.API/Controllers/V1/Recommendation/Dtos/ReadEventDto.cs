namespace RecommendationService.API.Controllers.V1.Recommendation.Dtos;

public class ReadGeolocationDto
{
    public float Lat { get; set; }
    public float Lng { get; set; }
}

public class ReadEventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset LastUpdateDate { get; set; }
    public bool IsPrivate { get; set; }
    public bool AdultsOnly { get; set; }
    public bool IsPaid { get; set; }
    public ReadUserDto Host { get; set; }
    public int MaxNumberOfAttendees { get; set; }
    public string Url { get; set; }
    public string? Description { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public ReadGeolocationDto GeoLocation { get; set; }
    public string AccessCode { get; set; }
    public string Category { get; set; }
    public IEnumerable<string> Keywords { get; set; }
    public IEnumerable<string>? Images { get; set; }
    public IEnumerable<ReadUserDto>? Attendees { get; set; }
}