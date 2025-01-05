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
using Serilog;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Options;
using SqlRepositoryBase.Core.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
builder.Services.AddOpenTelemetryTracing(builder.Configuration);

var assemblies = AppDomain.CurrentDomain.GetAssemblies();

// configure AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(assemblies));

builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));

// configure database
builder.Services.ConfigureConnectionStringFromAppSettings(builder.Configuration.GetSection("PostgreSql"))
        .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
        .ConfigurePostgreSql();

builder.Services.AddMassTransit(
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
builder.Services.AddTransientWithProxy<IAntiClownDataApiClient>(
    serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
);

// temp manual build VersionedSqlRepository
builder.Services.AddTransientWithProxy<IVersionedSqlRepository<EconomyStorageElement>, VersionedSqlRepository<EconomyStorageElement>>();
builder.Services.AddTransientWithProxy<IVersionedSqlRepository<ShopStorageElement>, VersionedSqlRepository<ShopStorageElement>>();
builder.Services.AddTransientWithProxy<IVersionedSqlRepository<ShopStatsStorageElement>, VersionedSqlRepository<ShopStatsStorageElement>>();

// configure repositories
builder.Services.AddTransientWithProxy<IUsersRepository, UsersRepository>();
builder.Services.AddTransientWithProxy<ITransactionsRepository, TransactionsRepository>();
builder.Services.AddTransientWithProxy<IEconomyRepository, EconomyRepository>();
builder.Services.AddTransientWithProxy<IItemsRepository, ItemsRepository>();
builder.Services.AddTransientWithProxy<IShopsRepository, ShopsRepository>();
builder.Services.AddTransientWithProxy<IShopItemsRepository, ShopItemsRepository>();
builder.Services.AddTransientWithProxy<IShopStatsRepository, ShopStatsRepository>();

// configure validators
builder.Services.AddTransientWithProxy<IItemsValidator, ItemsValidator>();
builder.Services.AddTransientWithProxy<IShopsValidator, ShopsValidator>();

// configure other stuff
builder.Services.AddTransientWithProxy<ITributeMessageProducer, TributeMessageProducer>();
builder.Services.AddTransientWithProxy<IScheduler, HangfireScheduler>();

// configure services
builder.Services.AddTransientWithProxy<IUsersService, UsersService>();
builder.Services.AddTransientWithProxy<INewUserService, NewUserService>();
builder.Services.AddTransientWithProxy<ITransactionsService, TransactionsService>();
builder.Services.AddTransientWithProxy<IEconomyService, EconomyService>();
builder.Services.AddTransientWithProxy<IItemsService, ItemsService>();
builder.Services.AddTransientWithProxy<IShopsService, ShopsService>();
builder.Services.AddTransientWithProxy<ITributeService, TributeService>();
builder.Services.AddTransientWithProxy<ILohotronRewardGenerator, LohotronRewardGenerator>();
builder.Services.AddTransientWithProxy<ILohotronService, LohotronService>();

// configure HangFire
builder.Services.AddHangfire(
    (serviceProvider, config) =>
        config.UsePostgreSqlStorage(serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>().Value.ConnectionString)
);
builder.Services.AddHangfireServer();

builder.Services.AddControllers().AddNewtonsoftJson(
    options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
    }
);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();
app.UseWebSockets();

app.UseSerilogRequestLogging();
app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.UseHangfireDashboard();

await app.RunAsync();