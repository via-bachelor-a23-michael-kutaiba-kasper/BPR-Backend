using EventManagementService.Test.Shared;

namespace EventManagementService.Test.JoinEvent;

[TestFixture]
public class JoinEventTests
{
    private readonly TestDataContext _context = new();
    
    [SetUp]
    public async Task Setup()
    {
        await _context.Clean();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.Clean();
    }
}