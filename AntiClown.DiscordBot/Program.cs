using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Configuration;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Consumers.Events.Common;
using AntiClown.DiscordBot.Consumers.Events.Daily;
using AntiClown.DiscordBot.Database;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.DiscordClientWrapper.BotBehaviour;
using AntiClown.DiscordBot.EmbedBuilders.F1PredictionsStats;
using AntiClown.DiscordBot.EmbedBuilders.GuessNumber;
using AntiClown.DiscordBot.EmbedBuilders.Inventories;
using AntiClown.DiscordBot.EmbedBuilders.Lottery;
using AntiClown.DiscordBot.EmbedBuilders.Parties;
using AntiClown.DiscordBot.EmbedBuilders.Rating;
using AntiClown.DiscordBot.EmbedBuilders.Releases;
using AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;
using AntiClown.DiscordBot.EmbedBuilders.Shops;
using AntiClown.DiscordBot.EmbedBuilders.Transactions;
using AntiClown.DiscordBot.EmbedBuilders.Transfusion;
using AntiClown.DiscordBot.EmbedBuilders.Tributes;
using AntiClown.DiscordBot.Interactivity.Repository;
using AntiClown.DiscordBot.Interactivity.Services.GuessNumber;
using AntiClown.DiscordBot.Interactivity.Services.Inventory;
using AntiClown.DiscordBot.Interactivity.Services.Lottery;
using AntiClown.DiscordBot.Interactivity.Services.Parties;
using AntiClown.DiscordBot.Interactivity.Services.Race;
using AntiClown.DiscordBot.Interactivity.Services.Shop;
using AntiClown.DiscordBot.Middlewares;
using AntiClown.DiscordBot.Options;
using AntiClown.DiscordBot.Releases.Repositories;
using AntiClown.DiscordBot.Releases.Services;
using AntiClown.DiscordBot.Roles.Repositories;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.DiscordBot.SlashCommands.Base.Middlewares;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Client.Configuration;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Bedge;
using AntiClown.Entertainment.Api.Dto.CommonEvents.GuessNumber;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Race;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using AntiClown.Entertainment.Api.Dto.DailyEvents.Announce;
using AntiClown.Entertainment.Api.Dto.DailyEvents.ResetsAndPayments;
using DSharpPlus;
using MassTransit;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SqlRepositoryBase.Configuration.Extensions;
using SqlRepositoryBase.Core.Options;
using TelemetryApp.Utilities.Extensions;
using TelemetryApp.Utilities.Middlewares;

namespace AntiClown.DiscordBot;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging();
        var telemetryApiUrl = builder.Configuration.GetSection("Telemetry").GetSection("ApiUrl").Value;
        var deployingEnvironment = builder.Configuration.GetValue<string>("DeployingEnvironment");
        builder.Services.ConfigureTelemetryClientWithLogger(
            "AntiClownBot" + (string.IsNullOrEmpty(deployingEnvironment) ? "" : $"_{deployingEnvironment}"),
            "DiscordBot",
            telemetryApiUrl
        );

        ConfigureOptions(builder);
        ConfigurePostgreSql(builder);
        BuildApiClients(builder);
        BuildDiscordServices(builder);
        BuildDiscordCaches(builder);
        BuildEmbedBuilders(builder);
        BuildInteractivityServices(builder);
        BuildCommonEventsConsumers(builder);
        BuildDailyEventsConsumers(builder);
        BuildMassTransit(builder);
        BuildSlashCommands(builder);

        builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.TypeNameHandling = TypeNameHandling.All);

        var app = builder.Build();

        var discordBotBehaviour = app.Services.GetRequiredService<IDiscordBotBehaviour>();
        await discordBotBehaviour.ConfigureAsync();

        var slashCommandsExecutor = app.Services.GetRequiredService<ICommandExecutor>();
        slashCommandsExecutor.AddMiddleware<LoggingMiddleware>();
        slashCommandsExecutor.AddMiddleware<ActualizeUsersCacheMiddleware>();
        slashCommandsExecutor.AddMiddleware<DeferredMessageMiddleware>();
        slashCommandsExecutor.AddMiddleware<CorrectChatCommandUsageMiddleware>();

        var discordClientWrapper = app.Services.GetRequiredService<IDiscordClientWrapper>();

        await discordClientWrapper.StartDiscordAsync();

        await Task.Delay(5 * 1000);
        await InitializeCachesAsync(app.Services);

        var releasesService = app.Services.GetRequiredService<IReleasesService>();
        await releasesService.NotifyIfNewVersionAvailableAsync();

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ServiceExceptionHandlingMiddleware>();
        app.UseEndpoints(endpoints => endpoints.MapControllers());

        await app.RunAsync();
    }

    private static void ConfigureOptions(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
        builder.Services.Configure<AntiClownApiConnectionOptions>(builder.Configuration.GetSection("AntiClownApi"));
        builder.Services.Configure<AntiClownEntertainmentApiConnectionOptions>(builder.Configuration.GetSection("AntiClownEntertainmentApi"));
        builder.Services.Configure<AntiClownDataApiConnectionOptions>(builder.Configuration.GetSection("AntiClownDataApi"));
        builder.Services.Configure<WebOptions>(builder.Configuration.GetSection("Web"));
    }

    private static void ConfigurePostgreSql(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureConnectionStringFromAppSettings(builder.Configuration.GetSection("PostgreSql"))
               .ConfigureDbContextFactory(connectionString => new DatabaseContext(connectionString))
               .ConfigurePostgreSql();

        builder.Services.AddSingleton<IDistributedLockProvider>(
            serviceProvider =>
            {
                var databaseOptions = serviceProvider.GetRequiredService<IOptions<AppSettingsDatabaseOptions>>();
                return new PostgresDistributedSynchronizationProvider(databaseOptions.Value.ConnectionString);
            }
        );

        builder.Services.AddTransient<IInteractivityRepository, InteractivityRepository>();
        builder.Services.AddTransient<IReleasesRepository, ReleasesRepository>();
        builder.Services.AddTransient<IRolesRepository, RolesRepository>();
    }

    private static void BuildApiClients(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl)
        );
        builder.Services.AddTransient<IAntiClownEntertainmentApiClient>(
            serviceProvider => AntiClownEntertainmentApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl
            )
        );
        builder.Services.AddTransient<IAntiClownDataApiClient>(
            serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
        );
    }

    private static void BuildDiscordServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DiscordClient>(
            serviceProvider =>
            {
                var settings = serviceProvider.GetService<IOptions<Settings>>()!.Value;
                var antiClownDataApiClient = serviceProvider.GetRequiredService<IAntiClownDataApiClient>();
                var logLevel = antiClownDataApiClient.Settings.ReadAsync(SettingsCategory.DiscordBot, "LogLevel").GetAwaiter().GetResult().Value;
                return new DiscordClient(
                    new DiscordConfiguration
                    {
                        Token = settings.ApiToken,
                        TokenType = TokenType.Bot,
                        MinimumLogLevel = Enum.TryParse<LogLevel>(logLevel, out var level) ? level : LogLevel.Information,
                        Intents = DiscordIntents.All,
                    }
                );
            }
        );
        builder.Services.AddTransient<IDiscordClientWrapper, DiscordClientWrapper.DiscordClientWrapper>();
        builder.Services.AddTransient<IDiscordBotBehaviour, DiscordBotBehaviour>();
    }

    private static void BuildDiscordCaches(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IUsersCache, UsersCache>();
        builder.Services.AddSingleton<IEmotesCache, EmotesCache>();
    }

    private static void BuildSlashCommands(WebApplicationBuilder builder)
    {
        var middlewareTypes = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(s => s.GetTypes())
                                       .Where(p => typeof(ICommandMiddleware).IsAssignableFrom(p) && !p.IsInterface);
        foreach (var middlewareType in middlewareTypes)
        {
            builder.Services.AddTransient(middlewareType);
            builder.Services.AddTransient(typeof(ICommandMiddleware), middlewareType);
        }

        builder.Services.AddSingleton<ICommandExecutor, CommandExecutor>();
    }

    private static void BuildEmbedBuilders(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ITributeEmbedBuilder, TributeEmbedBuilder>();
        builder.Services.AddTransient<ITransactionsEmbedBuilder, TransactionsEmbedBuilder>();
        builder.Services.AddTransient<IInventoryEmbedBuilder, InventoryEmbedBuilder>();
        builder.Services.AddTransient<IShopEmbedBuilder, ShopEmbedBuilder>();
        builder.Services.AddTransient<IGuessNumberEmbedBuilder, GuessNumberEmbedBuilder>();
        builder.Services.AddTransient<ITransfusionEmbedBuilder, TransfusionEmbedBuilder>();
        builder.Services.AddTransient<ILotteryEmbedBuilder, LotteryEmbedBuilder>();
        builder.Services.AddTransient<IPartyEmbedBuilder, PartyEmbedBuilder>();
        builder.Services.AddTransient<IRatingEmbedBuilder, RatingEmbedBuilder>();
        builder.Services.AddTransient<ILootBoxEmbedBuilder, LootBoxEmbedBuilder>();
        builder.Services.AddTransient<IReleaseEmbedBuilder, ReleaseEmbedBuilder>();
        builder.Services.AddTransient<IF1PredictionStatsEmbedBuilder, F1PredictionStatsEmbedBuilder>();
    }

    private static void BuildInteractivityServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IInventoryService, InventoryService>();
        builder.Services.AddTransient<IShopService, ShopService>();
        builder.Services.AddTransient<IGuessNumberEventService, GuessNumberEventService>();
        builder.Services.AddTransient<ILotteryService, LotteryService>();
        builder.Services.AddTransient<IRemoveCoolDownsEmbedBuilder, RemoveCoolDownsEmbedBuilder>();
        builder.Services.AddTransient<IPartiesService, PartiesService>();
        builder.Services.AddTransient<IRaceService, RaceService>();
        builder.Services.AddTransient<ICurrentReleaseProvider, CurrentReleaseProvider>();
        builder.Services.AddTransient<IReleasesService, ReleasesService>();
    }

    private static void BuildCommonEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICommonEventConsumer<GuessNumberEventDto>, GuessNumberEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<LotteryEventDto>, LotteryEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<RaceEventDto>, RaceEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<RemoveCoolDownsEventDto>, RemoveCoolDownsEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<TransfusionEventDto>, TransfusionEventConsumer>();
        builder.Services.AddTransient<ICommonEventConsumer<BedgeEventDto>, BedgeEventConsumer>();
    }

    private static void BuildDailyEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IDailyEventConsumer<AnnounceEventDto>, AnnounceEventConsumer>();
        builder.Services.AddTransient<IDailyEventConsumer<ResetsAndPaymentsEventDto>, ResetsAndPaymentsConsumer>();
    }

    private static void BuildMassTransit(WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(
            massTransitConfiguration =>
            {
                massTransitConfiguration.AddConsumers(AppDomain.CurrentDomain.GetAssemblies());
                massTransitConfiguration.SetKebabCaseEndpointNameFormatter();
                massTransitConfiguration.UsingRabbitMq(
                    (context, rabbitMqConfiguration) =>
                    {
                        var rabbitMqOptions = context.GetService<IOptions<RabbitMqOptions>>()!.Value;
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
    }

    private static async Task InitializeCachesAsync(IServiceProvider serviceProvider)
    {
        var usersCache = serviceProvider.GetRequiredService<IUsersCache>();
        var emotesCache = serviceProvider.GetRequiredService<IEmotesCache>();

        await Task.WhenAll(usersCache.InitializeAsync(), emotesCache.InitializeAsync());
    }
}