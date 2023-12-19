using SqlRepositoryBase.Core.Options;

namespace AntiClown.Api.Core.IntegrationTests.Mocks;

public class TestsConnectionStringProvider : IConnectionStringProvider
{
    public string GetConnectionString()
    {
        var variables = Environment.GetEnvironmentVariables();
        return Environment.GetEnvironmentVariable("AntiClown.Tests.PostgreSqlConnectionString")
               ?? throw new InvalidOperationException("No ConnectionString was provided");
    }
}