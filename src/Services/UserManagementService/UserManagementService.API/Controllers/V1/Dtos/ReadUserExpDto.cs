namespace UserManagementService.API.Controllers.V1.Dtos;

public class ReadUserExpDto
{
    public long TotalExp { get; set; }
    public ReadLevelDto Level { get; set; }
    public IReadOnlyCollection<ReadExpProgressEntryDto> ExpProgressHistory { get; set; }
}

public class ReadLevelDto
{
    public int Value { get; set; }
    public long MaxExp { get; set; }
    public long MinExp { get; set; }
    public string Name { get; set; } = "";
}

public class ReadExpProgressEntryDto
{
    public DateTimeOffset Timestamp { get; set; }
    public long ExpGained { get; set; }
}