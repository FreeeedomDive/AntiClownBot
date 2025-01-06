using AntiClown.Api.Client;
using AntiClown.Api.Client.Configuration;
using AntiClown.Core.OpenTelemetry;
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
using AntiClown.DiscordBot.EmbedBuilders.F1Predictions;
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
using AntiClown.DiscordBot.Utility.Locks;
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
using DSharpPlus.VoiceNext;
using MassTransit;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using SqlRepositoryBase.Configuration.Extensions;

namespace AntiClown.DiscordBot;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
        builder.Services.AddOpenTelemetryTracing(builder.Configuration);

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

        builder.Services.AddControllers().AddNewtonsoftJson(
            options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.All;
            }
        );

        var app = builder.Build();

        var discordBotBehaviour = app.Services.GetRequiredService<IDiscordBotBehaviour>();
        await discordBotBehaviour.ConfigureAsync();

        var slashCommandsExecutor = app.Services.GetRequiredService<ICommandExecutor>();
        slashCommandsExecutor.AddMiddleware<LoggingMiddleware>();
        slashCommandsExecutor.AddMiddleware<ActualizeUsersCacheMiddleware>();
        slashCommandsExecutor.AddMiddleware<DeferredMessageMiddleware>();
        slashCommandsExecutor.AddMiddleware<CorrectChatCommandUsageMiddleware>();
        slashCommandsExecutor.AddMiddleware<CheckRightsMiddleware>();

        var discordClientWrapper = app.Services.GetRequiredService<IDiscordClientWrapper>();

        await discordClientWrapper.StartDiscordAsync();

        await Task.Delay(5 * 1000);
        await InitializeCachesAsync(app.Services);

        var releasesService = app.Services.GetRequiredService<IReleasesService>();
        await releasesService.NotifyIfNewVersionAvailableAsync();

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseWebSockets();

        app.UseSerilogRequestLogging();
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

        builder.Services.AddTransientWithProxy<ILocker, Locker>();
        builder.Services.AddTransientWithProxy<IInteractivityRepository, InteractivityRepository>();
        builder.Services.AddTransientWithProxy<IReleasesRepository, ReleasesRepository>();
        builder.Services.AddTransientWithProxy<IRolesRepository, RolesRepository>();
    }

    private static void BuildApiClients(WebApplicationBuilder builder)
    {
        builder.Services.AddTransientWithProxy<IAntiClownApiClient>(
            serviceProvider => AntiClownApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownApiConnectionOptions>>().Value.ServiceUrl)
        );
        builder.Services.AddTransientWithProxy<IAntiClownEntertainmentApiClient>(
            serviceProvider => AntiClownEntertainmentApiClientProvider.Build(
                serviceProvider.GetRequiredService<IOptions<AntiClownEntertainmentApiConnectionOptions>>().Value.ServiceUrl
            )
        );
        builder.Services.AddTransientWithProxy<IAntiClownDataApiClient>(
            serviceProvider => AntiClownDataApiClientProvider.Build(serviceProvider.GetRequiredService<IOptions<AntiClownDataApiConnectionOptions>>().Value.ServiceUrl)
        );
    }

    private static void BuildDiscordServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<DiscordClient>(
            serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<Settings>>().Value;
                var antiClownDataApiClient = serviceProvider.GetRequiredService<IAntiClownDataApiClient>();
                var logLevel = antiClownDataApiClient.Settings.ReadAsync(SettingsCategory.DiscordBot, "LogLevel").GetAwaiter().GetResult().Value;
                var client = new DiscordClient(
                    new DiscordConfiguration
                    {
                        Token = settings.ApiToken,
                        TokenType = TokenType.Bot,
                        MinimumLogLevel = Enum.TryParse<LogLevel>(logLevel, out var level) ? level : LogLevel.Information,
                        Intents = DiscordIntents.All,
                    }
                );
                client.UseVoiceNext();
                return client;
            }
        );
        builder.Services.AddSingleton<VoiceNextExtension>(provider =>
        {
            var discordClient = provider.GetRequiredService<DiscordClient>();
            return discordClient.GetVoiceNext();
        });
        builder.Services.AddTransientWithProxy<IDiscordClientWrapper, DiscordClientWrapper.DiscordClientWrapper>();
        builder.Services.AddTransientWithProxy<IDiscordBotBehaviour, DiscordBotBehaviour>();
    }

    private static void BuildDiscordCaches(WebApplicationBuilder builder)
    {
        builder.Services.AddSingletonWithProxy<IUsersCache, UsersCache>();
        builder.Services.AddSingletonWithProxy<IEmotesCache, EmotesCache>();
    }

    private static void BuildSlashCommands(WebApplicationBuilder builder)
    {
        var middlewareTypes = AppDomain.CurrentDomain.GetAssemblies()
                                       .SelectMany(s => s.GetTypes())
                                       .Where(p => typeof(ICommandMiddleware).IsAssignableFrom(p) && !p.IsInterface);
        foreach (var middlewareType in middlewareTypes)
        {
            builder.Services.AddTransient(middlewareType);
            builder.Services.AddTransientWithProxy(typeof(ICommandMiddleware), middlewareType);
        }

        builder.Services.AddSingletonWithProxy<ICommandExecutor, CommandExecutor>();
    }

    private static void BuildEmbedBuilders(WebApplicationBuilder builder)
    {
        builder.Services.AddTransientWithProxy<ITributeEmbedBuilder, TributeEmbedBuilder>();
        builder.Services.AddTransientWithProxy<ITransactionsEmbedBuilder, TransactionsEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IInventoryEmbedBuilder, InventoryEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IShopEmbedBuilder, ShopEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IGuessNumberEmbedBuilder, GuessNumberEmbedBuilder>();
        builder.Services.AddTransientWithProxy<ITransfusionEmbedBuilder, TransfusionEmbedBuilder>();
        builder.Services.AddTransientWithProxy<ILotteryEmbedBuilder, LotteryEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IPartyEmbedBuilder, PartyEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IRatingEmbedBuilder, RatingEmbedBuilder>();
        builder.Services.AddTransientWithProxy<ILootBoxEmbedBuilder, LootBoxEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IReleaseEmbedBuilder, ReleaseEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IF1PredictionStatsEmbedBuilder, F1PredictionStatsEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IF1PredictionsEmbedBuilder, F1PredictionsEmbedBuilder>();
    }

    private static void BuildInteractivityServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransientWithProxy<IInventoryService, InventoryService>();
        builder.Services.AddTransientWithProxy<IShopService, ShopService>();
        builder.Services.AddTransientWithProxy<IGuessNumberEventService, GuessNumberEventService>();
        builder.Services.AddTransientWithProxy<ILotteryService, LotteryService>();
        builder.Services.AddTransientWithProxy<IRemoveCoolDownsEmbedBuilder, RemoveCoolDownsEmbedBuilder>();
        builder.Services.AddTransientWithProxy<IPartiesService, PartiesService>();
        builder.Services.AddTransientWithProxy<IRaceService, RaceService>();
        builder.Services.AddTransientWithProxy<ICurrentReleaseProvider, CurrentReleaseProvider>();
        builder.Services.AddTransientWithProxy<IReleasesService, ReleasesService>();
    }

    private static void BuildCommonEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<GuessNumberEventDto>, GuessNumberEventConsumer>();
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<LotteryEventDto>, LotteryEventConsumer>();
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<RaceEventDto>, RaceEventConsumer>();
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<RemoveCoolDownsEventDto>, RemoveCoolDownsEventConsumer>();
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<TransfusionEventDto>, TransfusionEventConsumer>();
        builder.Services.AddTransientWithProxy<ICommonEventConsumer<BedgeEventDto>, BedgeEventConsumer>();
    }

    private static void BuildDailyEventsConsumers(WebApplicationBuilder builder)
    {
        builder.Services.AddTransientWithProxy<IDailyEventConsumer<AnnounceEventDto>, AnnounceEventConsumer>();
        builder.Services.AddTransientWithProxy<IDailyEventConsumer<ResetsAndPaymentsEventDto>, ResetsAndPaymentsConsumer>();
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
    }

    private static async Task InitializeCachesAsync(IServiceProvider serviceProvider)
    {
        var usersCache = serviceProvider.GetRequiredService<IUsersCache>();
        var emotesCache = serviceProvider.GetRequiredService<IEmotesCache>();

        await Task.WhenAll(usersCache.InitializeAsync(), emotesCache.InitializeAsync());
    }
}