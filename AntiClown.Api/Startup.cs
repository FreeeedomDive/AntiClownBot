using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using AutoMapper;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Repository;

namespace AntiClown.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        // configure database
        var postgresSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgresSection);
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();

        // temp manual build VersionedSqlRepository
        services.AddTransient<
            IVersionedSqlRepository<EconomyStorageElement>,
            VersionedSqlRepository<EconomyStorageElement>
        >();

        // configure repositories
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ITransactionsRepository, TransactionsRepository>();
        services.AddTransient<IEconomyRepository, EconomyRepository>();
        services.AddTransient<IItemsRepository, ItemsRepository>();
        
        // configure validators
        services.AddTransient<IItemsValidator, ItemsValidator>();

        // configure services
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<INewUserService, NewUserService>();
        services.AddTransient<ITransactionsService, TransactionsService>();
        services.AddTransient<IEconomyService, EconomyService>();
        services.AddTransient<IItemsService, ItemsService>();

        // configure HangFire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(postgresSection["ConnectionString"])
        );

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireServer();
        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}