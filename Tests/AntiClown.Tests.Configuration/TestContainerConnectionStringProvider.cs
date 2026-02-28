using SqlRepositoryBase.Core.Options;
using Testcontainers.PostgreSql;

namespace AntiClown.Tests.Configuration;

public class TestContainerConnectionStringProvider(PostgreSqlContainer container) : IConnectionStringProvider
{
    public string GetConnectionString() => container.GetConnectionString();
}
