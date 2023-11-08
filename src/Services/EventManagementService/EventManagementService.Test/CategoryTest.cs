using Dapper;
using EventManagementService.Application.FetchAllPublicEvents.Model;
using EventManagementService.Test.Shared;
using Npgsql;

namespace EventManagementService.Test;

[TestFixture]
public class CategoryTest
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

    [TestCase("Testing")]
    [TestCase("Testing 2")]
    [TestCase("Testing 3")]
    public void InsertCategory(string name)
    {
        var statement = """
                        INSERT INTO category(name) VALUES (@name)
                        """;
        using var connection = new NpgsqlConnection(_context.ConnectionString);
        connection.Open();
        var parameters = new {@name=name }; 
        connection.Execute(statement, parameters);

        var allCategoriesStatement = """
                            SELECT id, name FROM category
                            """;
        var allCategoriesResultSet = connection.Query<Category>(allCategoriesStatement).ToList();
        Assert.That(allCategoriesResultSet, Has.Count.EqualTo(1));
    }
}