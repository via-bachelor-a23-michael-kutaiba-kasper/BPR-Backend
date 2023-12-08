using System.ComponentModel;

namespace UserManagementService.Domain.Models;

public enum UserAchievement
{
    [Description("Un Assigned")] UnAssigned = 0,
    [Description("Canary 1")] Canary1 = 1,
    [Description("Canary 2")] Canary2 = 2,
    [Description("Canary 3")] Canary3 = 3,
    [Description("Owl 1")] Owl1 = 4,
    [Description("Owl 2")] Owl2 = 5,
    [Description("Owl 3")] Owl = 6,
    [Description("Peacock 1")] Peacock1 = 7,
    [Description("Peacock 2")] Peacock2 = 8,
    [Description("Peacock 3")] Peacock3 = 9,
    [Description("Bear 1")] Bear1 = 10,
    [Description("Bear 2")] Bear2 = 11,
    [Description("Bear 3")] Bear3 = 12,
    [Description("Butterfly 1")] Butterfly = 13,
    [Description("Butterfly 2")] Butterfly2 = 14,
    [Description("Butterfly 3")] Butterfly3 = 15,
    [Description("Cheetah 1")] Cheetah1 = 16,
    [Description("Cheetah 2")] Cheetah2 = 17,
    [Description("Cheetah 3")] Cheetah3 = 18,
    [Description("Monkey 1")] Monkey1 = 19,
    [Description("Monkey 2")] Monkey2 = 20,
    [Description("Monkey 3")] Monkey3 = 21
}