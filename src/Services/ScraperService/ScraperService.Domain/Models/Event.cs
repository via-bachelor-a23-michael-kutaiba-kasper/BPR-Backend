namespace ScraperService.Domain.Models;

public record Event
(
    Guid Id,
    string Title,
    string Url,
    Location Location,
    string? Description,
    IReadOnlyCollection<string>? Keywords = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null,
    IReadOnlyCollection<string>? Images = null,
    int? NumberOfAttendees = null,
    bool? IsAdultOnly = null,
    bool? IsReoccurring = null,
    decimal? Price = null,
    string? Currency = null
)
{
    public required Guid Id { get; set; } = Id;
    public required string Title { get; set; } = Title;
    public required string Url { get; set; } = Url;
    public required Location Location { get; set; } = Location;
    public string? Description { get; set; } = Description;
    public IReadOnlyCollection<string>? Keywords { get; set; } = Keywords;
    public DateTimeOffset? StartDate { get; set; } = StartDate;
    public DateTimeOffset? EndDate { get; set; } = EndDate;

    public IReadOnlyCollection<string>?
        Images { get; set; } =
        Images; // make sure to save the main image first in the list

    public int? NumberOfAttendees { get; set; } = NumberOfAttendees;
    public bool? IsAdultOnly { get; set; } = IsAdultOnly;
    public bool? IsReoccurring { get; set; } = IsReoccurring;
    public decimal? Price { get; set; } = Price;
    public string? Currency { get; set; } = Currency;
}

public record Location
(
    string Country,
    string City,
    string PostalCode,
    string StreetName,
    string? StreetNumber = null,
    string? HouseNumber = null,
    string? Floor = null
)
{
    public required string Country { get; set; } = Country;
    public required string City { get; set; } = City;
    public required string PostalCode { get; set; } = PostalCode;
    public required string StreetName { get; set; } = StreetName;
    public string? StreetNumber { get; set; } = StreetNumber;
    public string? HouseNumber { get; set; } = HouseNumber;
    public string? Floor { get; set; } = Floor;
}