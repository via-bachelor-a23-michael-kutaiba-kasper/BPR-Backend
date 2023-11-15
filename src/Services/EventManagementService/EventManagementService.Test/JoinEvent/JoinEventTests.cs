using EventManagementService.Infrastructure;
using EventManagementService.Test.Shared;

namespace EventManagementService.Test.JoinEvent;

[TestFixture]
public class JoinEventTests
{
    private readonly TestDataContext _context = new();
    private readonly ConnectionStringManager _connectionStringManager = new();
    
    [SetUp]
    public async Task Setup()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }

    [TearDown]
    public async Task TearDown()
    {
        _context.ConnectionString = _connectionStringManager.GetConnectionString();
        await _context.Clean();
    }
    
    
}