namespace EventManagementService.Domain.Models;

public class Location
{
    public string Country{ get; set; }
    public string City { get; set; }
    public string PostalCode{ get; set; }
    public string StreetNumber{ get; set; }
    public string HouseNumber{ get; set; }
    public string? Floor{ get; set; }
}