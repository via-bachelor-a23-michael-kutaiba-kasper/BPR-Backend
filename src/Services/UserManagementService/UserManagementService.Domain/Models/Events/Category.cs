using System.ComponentModel;
using UserManagementService.Domain.Util;

namespace UserManagementService.Domain.Models.Events;

public enum Category
{
    [Description("Un Assigned")] UnAssigned = 0,

    [CategoryGroup("Music and Performing Arts")] [Description("Concerts")]
    Concerts = 1,

    [CategoryGroup("Music and Performing Arts")] [Description("Festivals")]
    Festivals = 2,

    [CategoryGroup("Learning and Development")] [Description("Conferences")]
    Conferences = 3,

    [CategoryGroup("Learning and Development")] [Description("Workshops")]
    Workshops = 4,

    [CategoryGroup("Learning and Development")] [Description("Seminars")]
    Seminars = 5,

    [CategoryGroup("Cultural and Artistic")] [Description("Arts and Culture")]
    ArtsAndCulture = 6,

    [CategoryGroup("Culinary and Drinks")] [Description("Food and Drink")]
    FoodAndDrink = 7,

    [CategoryGroup("Social and Community")] [Description("Charity and Fundraising")]
    CharityAndFundraising = 8,

    [CategoryGroup("Health and Wellness")] [Description("Health and Wellness")]
    HealthAndWellness = 9,

    [CategoryGroup("Learning and Development")] [Description("Technology")]
    Technology = 10,

    [CategoryGroup("Learning and Development")] [Description("Business and Entrepreneurship")]
    BusinessAndEntrepreneurship = 11,

    [CategoryGroup("Learning and Development")] [Description("Education")]
    Education = 12,

    [CategoryGroup("Social and Community")] [Description("Family and Kids")]
    FamilyAndKids = 13,

    [CategoryGroup("Social and Community")] [Description("Outdoor and Adventure")]
    OutdoorAndAdventure = 14,

    [CategoryGroup("Cultural and Artistic")] [Description("Comedy")]
    Comedy = 15,

    [CategoryGroup("Cultural and Artistic")] [Description("Film and Cinema")]
    FilmAndCinema = 16,

    [CategoryGroup("Music and Performing Arts")] [Description("Music")]
    Music = 17,

    [CategoryGroup("Music and Performing Arts")] [Description("Performing Arts")]
    PerformingArts = 18,

    [CategoryGroup("Cultural and Artistic")] [Description("Classic Literature")]
    ClassicLiterature = 19,

    [CategoryGroup("Culinary and Drinks")] [Description("Drinks")]
    Drinks = 20,

    [CategoryGroup("Health and Wellness")] [Description("Fitness and Workouts")]
    FitnessAndWorkouts = 21,

    [CategoryGroup("Culinary and Drinks")] [Description("Foods")]
    Foods = 22,

    [CategoryGroup("Recreation and Hobbies")] [Description("Games")]
    Games = 23,

    [CategoryGroup("Recreation and Hobbies")] [Description("Gardening")]
    Gardening = 24,

    [CategoryGroup("Health and Wellness")] [Description("Healthy Living and Self Care")]
    HealthyLivingAndSelfCare = 25,

    [CategoryGroup("Recreation and Hobbies")] [Description("Home and Garden")]
    HomeAndGarden = 26,

    [CategoryGroup("Social and Community")] [Description("Parties")]
    Parties = 27,

    [CategoryGroup("Social and Community")] [Description("Religions")]
    Religions = 28,

    [CategoryGroup("Recreation and Hobbies")] [Description("Shopping")]
    Shopping = 29,

    [CategoryGroup("Social and Community")] [Description("Social Issues")]
    SocialIssues = 30,

    [CategoryGroup("Health and Wellness")] [Description("Sports")]
    Sports = 31,

    [CategoryGroup("Cultural and Artistic")] [Description("Theater")]
    Theater = 32
}