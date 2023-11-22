namespace EventManagementService.Domain.Models.Events;

public class Location
{
    public string Country{ get; set; }
    public string City { get; set; }
    public string PostalCode{ get; set; }
    public string StreetName{ get; set; }
    public string StreetNumber{ get; set; }
    public string? HouseNumber{ get; set; }
    public string? SubPremise{ get; set; }
    public GeoLocation GeoLocation { get; set; }
}

public class GeoLocation
{
    public float Lat { get; set; }
    public float Lng { get; set; }
}