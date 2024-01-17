using System.Collections.Concurrent;

namespace UserManagementService.Infrastructure;

public interface IConnectionStringManager
{
    public string GetConnectionString();
}

public class ConnectionStringManager: IConnectionStringManager
{
    /// <summary>
    /// Indicates whether we are running in CI environment.
    /// CI system usually set an environment variable CI to true automatically.
    /// If not set automatically, we can set this explicitly in our CI/CD flow definitions.
    /// </summary>
    private readonly bool _ci = false;

    /// <summary>
    /// Determines what environment the application is running in.
    /// Should be either CI, LOCAL or PRODUCTION.
    /// </summary>
    /// <remarks>
    /// If the CI environment variable is set, we assume the environment is CI
    /// regardless of the what the value of DEPLOYMENT_ENVIRONMENT is set to.
    /// </remarks>
    private readonly string _deploymentEnvironment;

    private readonly IDictionary<string, string> _connectionStrings = new ConcurrentDictionary<string, string>();
    
    public ConnectionStringManager()
    {
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres";
        
        _ci = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));
        // Values should be "LOCAL", "PRODUCTION", "CI" or "LOCAL_CONTAINER"
        _deploymentEnvironment = Environment.GetEnvironmentVariable("DEPLOYMENT_ENVIRONMENT") ?? "LOCAL";
        Console.WriteLine($"Deployment Environment: {_deploymentEnvironment}");
        
        // NOTE: This is only here due to it being a school project. Otherwise we would use a vault or pass by environment variable.
        _connectionStrings.Add($"PRODUCTION", $"Server=35.234.109.192 ;Port=5432;Database=postgres;User Id=postgres;Password={password};SearchPath=user_progress;");
        _connectionStrings.Add($"CI", $"Server=34.107.56.249 ;Port=5432;Database=postgres;User Id=postgres;Password={password};SearchPath=user_progress;");
        _connectionStrings.Add($"LOCAL", $"Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;SearchPath=user_progress;");
        _connectionStrings.Add($"LOCAL_CONTAINER", $"Server=usermanagement_postgres;Port=5432;Database=postgres;User Id=postgres;Password=postgres;SearchPath=user_progress;");
    }

    public string GetConnectionString()
    {
        // Prioritize CI 
        if (_ci)
        {
            return _connectionStrings["CI"];
        }
        
        if (_connectionStrings.TryGetValue(_deploymentEnvironment, out var connectionString))
        {
            return connectionString;
        }

        throw new Exception("Failed to determine the current deployment environment");
    }
}