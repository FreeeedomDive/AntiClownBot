using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AutoFixture;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace IntegrationTests;

public class IntegrationTestsBase
{
    [OneTimeSetUp]
    public void Setup()
    {
        // Build all dependencies
        Fixture = new Fixture();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(assemblies));
        var mapper = mapperConfiguration.CreateMapper();

        var databaseContext = new DatabaseContext();

        var usersSqlRepository = new SqlRepository<UserStorageElement>(databaseContext);
        var usersRepository = new UsersRepository(usersSqlRepository, mapper);

        UsersService = new UsersService(usersRepository);
        NewUserService = new NewUserService(usersRepository, mapper);
    }
    
    protected IUsersService UsersService { get; private set; }
    protected INewUserService NewUserService { get; private set; }
    protected IFixture Fixture { get; private set; }
}