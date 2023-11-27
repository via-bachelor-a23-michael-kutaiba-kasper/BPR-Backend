using System.ComponentModel;

namespace EventManagementService.Domain.Models.Events;

public enum Category
{
    [Description("Concerts")] Concerts = 1,

    [Description("Festivals")] Festivals = 2,

    [Description("Conferences")] Conferences = 3,

    [Description("Workshops")] Workshops = 4,

    [Description("Seminars")] Seminars = 5,

    [Description("Arts and Culture")] ArtsAndCulture = 6,

    [Description("Food and Drink")] FoodAndDrink = 7,

    [Description("Charity and Fundraising")]
    CharityAndFundraising = 8,

    [Description("Health and Wellness")] HealthAndWellness = 9,

    [Description("Technology")] Technology = 10,

    [Description("Business and Entrepreneurship")]
    BusinessAndEntrepreneurship = 11,

    [Description("Education")] Education = 12,

    [Description("Family and Kids")] FamilyAndKids = 13,

    [Description("Outdoor and Adventure")] OutdoorAndAdventure = 14,

    [Description("Comedy")] Comedy = 15,

    [Description("Film and Cinema")] FilmAndCinema = 16,

    [Description("Music")] Music = 17,

    [Description("Performing Arts")] PerformingArts = 18,

    [Description("Classic Literature")] ClassicLiterature = 19,

    [Description("Drinks")] Drinks = 20,

    [Description("Fitness and Workouts")] FitnessAndWorkouts = 21,

    [Description("Foods")] Foods = 22,

    [Description("Games")] Games = 23,

    [Description("Gardening")] Gardening = 24,

    [Description("Healthy Living and Self Care")]
    HealthyLivingAndSelfCare = 25,

    [Description("Home and Garden")] HomeAndGarden = 26,

    [Description("Parties")] Parties = 27,

    [Description("Religions")] Religions = 28,

    [Description("Shopping")] Shopping = 29,

    [Description("Social Issues")] SocialIssues = 30,

    [Description("Sports")] Sports = 31,

    [Description("Theater")] Theater = 32
}