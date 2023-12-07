using System.ComponentModel;

namespace RecommendationService.Domain.Events;

public enum Keyword
{
    [Description("Un Assigned")] UnAssigned = 0,
    
    [Description("Live Music")] LiveMusic = 1,

    [Description("Dance")] Dance = 2,

    [Description("Art Exhibition")] ArtExhibition = 3,

    [Description("Science")] Science = 4,

    [Description("Coding")] Coding = 5,

    [Description("Gaming")] Gaming = 6,

    [Description("Yoga")] Yoga = 7,

    [Description("Meditation")] Meditation = 8,

    [Description("Fitness")] Fitness = 9,

    [Description("Food Tasting")] FoodTasting = 10,

    [Description("Networking")] Networking = 11,

    [Description("Fashion Show")] FashionShow = 12,

    [Description("Photography")] Photography = 13,

    [Description("Charity Auction")] CharityAuction = 14,

    [Description("Marathon")] Marathon = 15,

    [Description("Theater")] Theater = 16,

    [Description("Poetry")] Poetry = 17,

    [Description("Wine Tasting")] WineTasting = 18,

    [Description("TEDx")] TEDx = 19,

    [Description("Hackathon")] Hackathon = 20,

    [Description("Rock")] Rock = 21,

    [Description("Jazz")] Jazz = 22,

    [Description("Hip Hop")] HipHop = 23,

    [Description("Electronic Music")] ElectronicMusic = 24,

    [Description("Classical Performance")] ClassicalPerformance = 25,

    [Description("Country Music")] CountryMusic = 26,

    [Description("Indie")] Indie = 27,

    [Description("Acoustic Jam Session")] AcousticJamSession = 28,

    [Description("Blues")] Blues = 29,

    [Description("Reggae Night")] ReggaeNight = 30,

    [Description("Beer")] Beer = 31,

    [Description("Wine Tasting (Beer)")] WineTastingBeer = 32,

    [Description("Cocktail")] Cocktail = 33,

    [Description("Coffee Tasting")] CoffeeTasting = 34,

    [Description("Whiskey Tasting")] WhiskeyTasting = 35,

    [Description("BBQ")] BBQ = 36,

    [Description("Vegan")] Vegan = 37,

    [Description("International Cuisine")] InternationalCuisine = 38,

    [Description("Football")] Football = 39,

    [Description("Basketball")] Basketball = 40,

    [Description("Marathon (Sports)")] MarathonSports = 41,

    [Description("Yoga (Sports)")] YogaSports = 42,

    [Description("Cycling")] Cycling = 43,

    [Description("Golf")] Golf = 44,

    [Description("Tennis")] Tennis = 45,

    [Description("Surfing")] Surfing = 46,

    [Description("Street Art")] StreetArt = 47,

    [Description("Photography Exhibition")]
    PhotographyExhibition = 48,

    [Description("Fashion Show (Arts)")] FashionShowArts = 49,

    [Description("Poetry Slam")] PoetrySlam = 50,

    [Description("Cultural Heritage")] CulturalHeritage = 51,

    [Description("Dance Performance")] DancePerformance = 52,

    [Description("Theater Play")] TheaterPlay = 53,

    [Description("Sculpture Expo")] SculptureExpo = 54
}