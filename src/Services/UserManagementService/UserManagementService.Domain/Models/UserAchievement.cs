using System.ComponentModel;
using UserManagementService.Domain.Util;

namespace UserManagementService.Domain.Models;

public enum UserAchievement
{
    [Description("Un Assigned")] UnAssigned = 0,

    [CategoryGroup("Music and Performing Arts")] [Description("Canary 1")]
    Canary1 = 1,

    [CategoryGroup("Music and Performing Arts")] [Description("Canary 2")]
    Canary2 = 2,

    [CategoryGroup("Music and Performing Arts")] [Description("Canary 3")]
    Canary3 = 3,

    [CategoryGroup("Learning and Development")] [Description("Owl 1")]
    Owl1 = 4,

    [CategoryGroup("Learning and Development")] [Description("Owl 2")]
    Owl2 = 5,

    [CategoryGroup("Learning and Development")] [Description("Owl 3")]
    Owl3 = 6,

    [CategoryGroup("Cultural and Artistic")] [Description("Peacock 1")]
    Peacock1 = 7,

    [CategoryGroup("Cultural and Artistic")] [Description("Peacock 2")]
    Peacock2 = 8,

    [CategoryGroup("Cultural and Artistic")] [Description("Peacock 3")]
    Peacock3 = 9,

    [CategoryGroup("Culinary and Drinks")] [Description("Bear 1")]
    Bear1 = 10,

    [CategoryGroup("Culinary and Drinks")] [Description("Bear 2")]
    Bear2 = 11,

    [CategoryGroup("Culinary and Drinks")] [Description("Bear 3")]
    Bear3 = 12,

    [CategoryGroup("Social and Community")] [Description("Butterfly 1")]
    Butterfly1 = 13,

    [CategoryGroup("Social and Community")] [Description("Butterfly 2")]
    Butterfly2 = 14,

    [CategoryGroup("Social and Community")] [Description("Butterfly 3")]
    Butterfly3 = 15,

    [CategoryGroup("Health and Wellness")] [Description("Cheetah 1")]
    Cheetah1 = 16,

    [CategoryGroup("Health and Wellness")] [Description("Cheetah 2")]
    Cheetah2 = 17,

    [CategoryGroup("Health and Wellness")] [Description("Cheetah 3")]
    Cheetah3 = 18,

    [CategoryGroup("Recreation and Hobbies")] [Description("Monkey 1")]
    Monkey1 = 19,

    [CategoryGroup("Recreation and Hobbies")] [Description("Monkey 2")]
    Monkey2 = 20,

    [CategoryGroup("Recreation and Hobbies")] [Description("Monkey 3")]
    Monkey3 = 21,
    [CategoryGroup("New comer")][Description("New comer")]
    NewComer = 22
}