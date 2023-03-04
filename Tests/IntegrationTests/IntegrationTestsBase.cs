using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Domain;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AutoFixture;
using AutoMapper;
using SqlRepositoryBase.Core.Repository;

namespace IntegrationTests;

public class IntegrationTestsBase
{
    [OneTimeSetUp]
    public void SetUpDependencies()
    {
        // Build all dependencies
        Fixture = new Fixture();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddMaps(assemblies));
        var mapper = mapperConfiguration.CreateMapper();

        var databaseContext = new DatabaseContext();

        var usersSqlRepository = new SqlRepository<UserStorageElement>(databaseContext);
        var usersRepository = new UsersRepository(usersSqlRepository, mapper);

        var economiesSqlRepository = new VersionedSqlRepository<EconomyStorageElement>(databaseContext);
        var economiesRepository = new EconomyRepository(economiesSqlRepository, mapper);

        var transactionsSqlRepository = new SqlRepository<TransactionStorageElement>(databaseContext);
        var transactionsRepository = new TransactionsRepository(transactionsSqlRepository, mapper);
        
        UsersService = new UsersService(usersRepository);
        TransactionsService = new TransactionsService(transactionsRepository);
        EconomyService = new EconomyService(economiesRepository, TransactionsService);
        NewUserService = new NewUserService(usersRepository, EconomyService, mapper);
    }

    [SetUp]
    public async Task SetUp()
    {
        var newUserId = await NewUserService.CreateNewUserAsync(new NewUser
        {
            DiscordId = CreateUniqueUlong()
        });
        User = await UsersService.ReadAsync(newUserId);
    }

    protected ulong CreateUniqueUlong()
    {
        return Enumerable.Range(1, 8)
            .Select(_ => Fixture.Create<ulong>())
            .Aggregate((accumulate, x) => accumulate * x);
    }
    
    protected IUsersService UsersService { get; private set; }
    protected INewUserService NewUserService { get; private set; }
    protected ITransactionsService TransactionsService { get; private set; }
    protected IEconomyService EconomyService { get; private set; }
    protected IFixture Fixture { get; private set; }
    protected User User { get; private set; }
}