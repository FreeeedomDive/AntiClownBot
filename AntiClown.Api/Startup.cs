using AntiClown.Api.Core.Database;
using AntiClown.Api.Core.Economies.Repositories;
using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Repositories;
using AntiClown.Api.Core.Inventory.Services;
using AntiClown.Api.Core.Options;
using AntiClown.Api.Core.Shops.Repositories.Items;
using AntiClown.Api.Core.Shops.Repositories.Shops;
using AntiClown.Api.Core.Shops.Repositories.Stats;
using AntiClown.Api.Core.Shops.Services;
using AntiClown.Api.Core.Transactions.Repositories;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Core.Users.Repositories;
using AntiClown.Api.Core.Users.Services;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
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

        var postgresSection = Configuration.GetSection("PostgreSql");
        services.Configure<DatabaseOptions>(postgresSection);
        var rabbitMqSection = Configuration.GetSection("RabbitMQ");

        // configure database
        services.AddTransient<DbContext, DatabaseContext>();
        services.AddDbContext<DatabaseContext>(ServiceLifetime.Transient, ServiceLifetime.Transient);
        services.ConfigurePostgreSql();
        services.AddMassTransit(massTransitConfiguration =>
        {
            massTransitConfiguration.UsingRabbitMq((context, rabbitMqConfiguration) =>
            {
                rabbitMqConfiguration.ConfigureEndpoints(context);
                rabbitMqConfiguration.Host(rabbitMqSection["Host"], "/", hostConfiguration =>
                {
                    hostConfiguration.Username(rabbitMqSection["Login"]);
                    hostConfiguration.Password(rabbitMqSection["Password"]);
                });
            });
        });

        // temp manual build VersionedSqlRepository
        services.AddTransient<IVersionedSqlRepository<EconomyStorageElement>, VersionedSqlRepository<EconomyStorageElement>>();
        services.AddTransient<IVersionedSqlRepository<ShopStorageElement>, VersionedSqlRepository<ShopStorageElement>>();
        services.AddTransient<IVersionedSqlRepository<ShopStatsStorageElement>, VersionedSqlRepository<ShopStatsStorageElement>>();

        // configure repositories
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ITransactionsRepository, TransactionsRepository>();
        services.AddTransient<IEconomyRepository, EconomyRepository>();
        services.AddTransient<IItemsRepository, ItemsRepository>();
        services.AddTransient<IShopsRepository, ShopsRepository>();
        services.AddTransient<IShopItemsRepository, ShopItemsRepository>();
        services.AddTransient<IShopStatsRepository, ShopStatsRepository>();
        
        // configure validators
        services.AddTransient<IItemsValidator, ItemsValidator>();
        services.AddTransient<IShopsValidator, ShopsValidator>();

        // configure services
        services.AddTransient<IUsersService, UsersService>();
        services.AddTransient<INewUserService, NewUserService>();
        services.AddTransient<ITransactionsService, TransactionsService>();
        services.AddTransient<IEconomyService, EconomyService>();
        services.AddTransient<IItemsService, ItemsService>();
        services.AddTransient<IShopsService, ShopsService>();

        // configure HangFire
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(postgresSection["ConnectionString"])
        );
        services.AddHangfireServer();

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}