﻿using AntiClown.Api.Core.Database;
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
using AntiClown.Api.Middlewares;
using AntiClown.Core.OpenTelemetry;
using AntiClown.Core.Schedules;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Options;
using SqlRepositoryBase.Core.Repository;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClown.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOpenTelemetryTracing(Configuration);

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // configure AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

        services.Configure<RabbitMqOptions>(Configuration.GetSection("RabbitMQ"));
        services.Configure<AntiClownDataApiConnectionOptions>(Configuration.GetSection("AntiClownDataApi"));
        var telemetryApiUrl = Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        var deployingEnvironment = Configuration.GetValue<string>("DeployingEnvironment");
        services.ConfigureTelemetryClientWithLogger(
            "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
            "Api",
            telemetryApiUrl
        );

        // configure database
        services.ConfigureConnectionStringFromAppSettings(Configuration.GetSection("PostgreSql"))
                .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
                .ConfigurePostgreSql();

        services.AddMassTransit(
            massTransitConfiguration =>
            {
                massTransitConfiguration.SetKebabCaseEndpointNameFormatter();
                massTransitConfiguration.UsingRabbitMq(
                    (context, rabbitMqConfiguration) =>
                    {
                        var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                        rabbitMqConfiguration.ConfigureEndpoints(context);
                        rabbitMqConfiguration.Host(
                            rabbitMqOptions.Host, "/", hostConfiguration =>
                            {
                                hostConfiguration.Username(rabbitMqOptions.Login);
                                hostConfiguration.Password(rabbitMqOptions.Password);
                            }
                        );
                    }
                );
            }
        );
        services.AddTransientWithProxy<IAntiClownDataApiClient>(
            serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
        );

        // temp manual build VersionedSqlRepository
        services.AddTransientWithProxy<IVersionedSqlRepository<EconomyStorageElement>, VersionedSqlRepository<EconomyStorageElement>>();
        services.AddTransientWithProxy<IVersionedSqlRepository<ShopStorageElement>, VersionedSqlRepository<ShopStorageElement>>();
        services.AddTransientWithProxy<IVersionedSqlRepository<ShopStatsStorageElement>, VersionedSqlRepository<ShopStatsStorageElement>>();

        // configure repositories
        services.AddTransientWithProxy<IUsersRepository, UsersRepository>();
        services.AddTransientWithProxy<ITransactionsRepository, TransactionsRepository>();
        services.AddTransientWithProxy<IEconomyRepository, EconomyRepository>();
        services.AddTransientWithProxy<IItemsRepository, ItemsRepository>();
        services.AddTransientWithProxy<IShopsRepository, ShopsRepository>();
        services.AddTransientWithProxy<IShopItemsRepository, ShopItemsRepository>();
        services.AddTransientWithProxy<IShopStatsRepository, ShopStatsRepository>();

        // configure validators
        services.AddTransientWithProxy<IItemsValidator, ItemsValidator>();
        services.AddTransientWithProxy<IShopsValidator, ShopsValidator>();

        // configure other stuff
        services.AddTransientWithProxy<ITributeMessageProducer, TributeMessageProducer>();
        services.AddTransientWithProxy<IScheduler, HangfireScheduler>();

        // configure services
        services.AddTransientWithProxy<IUsersService, UsersService>();
        services.AddTransientWithProxy<INewUserService, NewUserService>();
        services.AddTransientWithProxy<ITransactionsService, TransactionsService>();
        services.AddTransientWithProxy<IEconomyService, EconomyService>();
        services.AddTransientWithProxy<IItemsService, ItemsService>();
        services.AddTransientWithProxy<IShopsService, ShopsService>();
        services.AddTransientWithProxy<ITributeService, TributeService>();
        services.AddTransientWithProxy<ILohotronRewardGenerator, LohotronRewardGenerator>();
        services.AddTransientWithProxy<ILohotronService, LohotronService>();

        // configure HangFire
        services.AddHangfire(
            (serviceProvider, config) =>
                config.UsePostgreSqlStorage(serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>().Value.ConnectionString)
        );
        services.AddHangfireServer();


        services.AddControllers().AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            }
        );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseHangfireDashboard();
    }

    public IConfiguration Configuration { get; }
}