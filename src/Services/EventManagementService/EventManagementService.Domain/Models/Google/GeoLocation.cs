namespace EventManagementService.Domain.Models.Google;

public record GeoLocation
(
    IReadOnlyCollection<Result> Results,
    string Status
)
{
    public required IReadOnlyCollection<Result> Results { get; set; } = Results;
    public required string Status { get; set; } = Status;
};

public record Result
(
    IReadOnlyCollection<AddressComponent> AddressComponents,
    string FormattedAddress,
    Geometry Geometry,
    bool PartialMatch,
    string PlaceId,
    IReadOnlyCollection<string> Types,
    PlusCode? PlusCode = null
)
{
    /// <summary>
    /// containing the separate components applicable to this address.
    /// Each address component typically contains the following fields
    /// The array of address components may contain more components than the formatted_address.
    /// The array does not necessarily include all the political entities that contain an address,
    /// apart from those included in the formatted_address.
    /// To retrieve all the political entities that contain a specific address, you should use reverse geocoding,
    /// passing the latitude/longitude of the address as a parameter to the request.
    /// The format of the response is not guaranteed to remain the same between requests.
    /// In particular, the number of address_components varies based on the address requested and can change over time
    /// for the same address.
    /// A component can change position in the array.
    /// The type of the component can change.
    /// A particular component may be missing in a later response.
    /// To handle the array of components, you should parse the response and select appropriate values via expressions.
    /// </summary>
    public required IReadOnlyCollection<AddressComponent> AddressComponents { get; set; } = AddressComponents;

    /// <summary>
    /// String containing the human-readable address of this location.
    /// Often this address is equivalent to the postal address. Note that some countries, such as the United Kingdom,
    /// do not allow distribution of true postal addresses due to licensing restrictions.
    /// The formatted address is logically composed of one or more address components.
    /// For example, the address "111 8th Avenue, New York, NY" consists of the following components: "111"
    /// (the street number), "8th Avenue" (the route), "New York" (the city) and "NY" (the US state).
    /// <remarks>
    /// Do not parse the formatted address programmatically.
    /// Instead you should use the individual address components, which the API response includes in addition to the
    /// formatted address field.
    /// </remarks>
    /// </summary>
    public required string FormattedAddress { get; set; } = FormattedAddress;

    /// <summary>
    /// Contains the location geo information
    /// </summary>
    public required Geometry Geometry { get; set; } = Geometry;

    /// <summary>
    ///  Indicates that the geocoder did not return an exact match for the original request,
    /// though it was able to match part of the requested address.
    /// You may wish to examine the original request for misspellings and/or an incomplete address.
    /// Partial matches most often occur for street addresses that do not exist within the locality you pass in the request.
    /// Partial matches may also be returned when a request matches two or more locations in the same locality.
    /// <remarks>
    /// Note that if a request includes a misspelled address component, the geocoding service may suggest an alternative address.
    /// Suggestions triggered in this way will also be marked as a partial match.
    /// </remarks>
    /// </summary>
    public required bool PartialMatch { get; set; } = PartialMatch;

    /// <summary>
    /// Is a unique identifier that can be used with other Google APIs.
    /// For example, you can use the place_id in a Places API request to get details of a local business,
    /// such as phone number, opening hours, user reviews, and more.
    /// </summary>
    public required string PlaceId { get; set; } = PlaceId;

    /// <summary>
    /// Indicates the type of the returned result.
    /// This array contains a set of zero or more tags identifying the type of feature returned in the result.
    /// or example, a geocode of "Chicago" returns "locality" which indicates that "Chicago" is a city, and also
    /// returns "political" which indicates it is a political entity.
    /// Components might have an empty types array when there are no known types for that address component.
    /// The API might add new type values as needed.
    /// For more information:https://developers.google.com/maps/documentation/geocoding/requests-geocoding#Types
    /// Where available, the API returns both the global code and compound code. However, if the result is in a remote
    /// location (for example, an ocean or desert) only the global code may be returned.
    /// </summary>
    public required IReadOnlyCollection<string> Types { get; set; } = Types;

    /// <summary>
    /// is an encoded location reference, derived from latitude and longitude coordinates,
    /// that represents an area: 1/8000th of a degree by 1/8000th of a degree (about 14m x 14m at the equator) or smaller.
    /// Plus codes can be used as a replacement for street addresses in places where addresses do not exist
    /// (where buildings are not numbered or streets are not named).
    /// The API does not always return plus codes.
    /// </summary>
    public PlusCode? PlusCode { get; set; } = PlusCode;
}

public record AddressComponent
(
    string LongName,
    IReadOnlyCollection<string> Types,
    string? ShortName = null
)
{
    /// <summary>
    /// Is the full text description or name of the address component as returned by the Geocoder.
    /// </summary>
    public required string LongName { get; set; } = LongName;

    /// <summary>
    /// Indicating the type of the address component.
    /// List of supported types: https://developers.google.com/maps/documentation/places/web-service/supported_types 
    /// </summary>
    public required IReadOnlyCollection<string> Types { get; set; } = Types;

    /// <summary>
    /// Is an abbreviated textual name for the address component, if available.
    /// For example, an address component for the state of Alaska may have a long_name of "Alaska" and a short_name of
    /// "AK" using the 2-letter postal abbreviation.
    /// </summary>
    public string? ShortName { get; set; } = ShortName;
}

public record Geometry
(
    Location Location,
    string LocationType,
    Viewport Viewport,
    Bounds? Bounds = null
)
{
    /// <summary>
    /// Contains the geocoded latitude, longitude value. For normal address lookups, this field is typically
    /// the most important.
    /// </summary>
    public required Location Location { get; set; } = Location;

    /// <summary>
    /// Stores additional data about the specified location.
    /// The following values are currently supported:
    /// "ROOFTOP" indicates that the returned result is a precise geocode for which we have location information
    /// accurate down to street address precision.
    /// "RANGE_INTERPOLATED" indicates that the returned result reflects an approximation (usually on a road)
    /// interpolated between two precise points (such as intersections). Interpolated results are generally
    /// returned when rooftop geocodes are unavailable for a street address.
    /// "GEOMETRIC_CENTER" indicates that the returned result is the geometric center of a result such as a polyline
    /// (for example, a street) or polygon (region).
    /// "APPROXIMATE" indicates that the returned result is approximate.
    /// </summary>
    public required string LocationType { get; set; } = LocationType;

    /// <summary>
    /// contains the recommended viewport for displaying the returned result, specified as two latitude,longitude
    /// values defining the southwest and northeast corner of the viewport bounding box. Generally the viewport is
    /// used to frame a result when displaying it to a user.
    /// </summary>
    public required Viewport Viewport { get; set; } = Viewport;

    /// <summary>
    /// (optionally returned) stores the bounding box which can fully contain the returned result.
    /// Note that these bounds may not match the recommended viewport.
    /// (For example, San Francisco includes the Farallon islands, which are technically part of the city,
    /// but probably should not be returned in the viewport.) 
    /// </summary>
    public Bounds? Bounds { get; set; } = Bounds;
}

public record Bounds
(
    Northeast Northeast,
    Southwest Southwest
)
{
    public required Northeast Northeast { get; set; } = Northeast;
    public required Southwest Southwest { get; set; } = Southwest;
}

public record Location
(
    string Lat,
    string Lng
)
{
    public string Lat { get; set; } = Lat;
    public string Lng { get; set; } = Lng;
}

public record Viewport
(
    Northeast Northeast,
    Southwest Southwest
)
{
    public Northeast Northeast { get; set; } = Northeast;
    public Southwest Southwest { get; set; } = Southwest;
}

public record Northeast
(
    string Lat,
    string Lng
)
{
    public string Lat { get; set; } = Lat;
    public string Lng { get; set; } = Lng;
}

public record Southwest
(
    string Lat,
    string Lng
)
{
    public string Lat { get; set; } = Lat;
    public string Lng { get; set; } = Lng;
}

public record PlusCode
(
    string CompoundCode,
    string? GlobalCode = null
)
{
    /// <summary>
    /// is a 6 character or longer local code with an explicit location (CWC8+R9, Mountain View, CA, USA).
    /// Do not programmatically parse this content.
    /// </summary>
    public required string CompoundCode { get; set; } = CompoundCode;

    /// <summary>
    /// Is a 4 character area code and 6 character or longer local code (849VCWC8+R9).
    /// </summary>
    public string? GlobalCode { get; set; } = GlobalCode;
}