using UserManagementService.Domain.Models.Events;

namespace UserManagementService.Application.V1.ProcessExpProgress.Dtos;

public class Attendance
{
    public string UserId{ get; set; }
    public Event Event { get; set; } 
}