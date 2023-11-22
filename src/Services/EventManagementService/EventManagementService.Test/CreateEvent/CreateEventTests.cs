using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;

namespace EventManagementService.Test.CreateEvent;

[TestFixture]
public class CreateEventTests
{
    private readonly TestDataContext _context = new();
    private readonly ConnectionStringManager _connectionStringManager = new();
}