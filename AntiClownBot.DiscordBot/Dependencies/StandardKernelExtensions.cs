using AntiClownApiClient;
using AntiClownDiscordBotVersion2.ApiEventFeed;
using AntiClownDiscordBotVersion2.Commands;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;
using AntiClownDiscordBotVersion2.Events;
using AntiClownDiscordBotVersion2.Events.NightEvents;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.ExceptionFilters;
using AntiClownDiscordBotVersion2.ExceptionFilters.Rules;
using AntiClownDiscordBotVersion2.MinecraftServer;
using AntiClownDiscordBotVersion2.Models;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Models.Lohotron;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.ServicesHealth;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Statistics.Daily;
using AntiClownDiscordBotVersion2.Statistics.Emotes;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;
using CommonServices.IpService;
using CommonServices.MinecraftServerService;
using DSharpPlus;
using Ninject;
using RestClient;
using TelemetryApp.Utilities.Extensions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace AntiClownDiscordBotVersion2.Dependencies;

public static class StandardKernelExtensions
{
    public static StandardKernel WithDiKernel(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IServiceProvider>().ToConstant(ninjectKernel);

        return ninjectKernel;
    }

    public static StandardKernel WithApplicationSettings(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IAppSettingsService>().To<AppSettingsService>();

        return ninjectKernel;
    }

    public static StandardKernel WithEventsSettings(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IEventSettingsService>().To<EventSettingsService>();

        return ninjectKernel;
    }

    public static StandardKernel WithGuildSettings(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IGuildSettingsService>().To<GuildSettingsService>();

        return ninjectKernel;
    }

    public static StandardKernel WithTelemetryClientWithLogger(this StandardKernel ninjectkernel)
    {
        var settings = ninjectkernel.Get<IAppSettingsService>().GetSettings();

        return ninjectkernel.ConfigureTelemetryClientWithLogger("AntiClownBot", "DiscordBot", settings.TelemetryApiUrl);
    }

    public static StandardKernel WithExceptionFilter(this StandardKernel ninjectKernel)
    {
        var rules = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IExceptionFilterRule).IsAssignableFrom(p))
            .Where(p => p != typeof(IExceptionFilterRule));

        foreach (var rule in rules)
        {
            ninjectKernel.Bind<IExceptionFilterRule>().To(rule);
        }

        ninjectKernel.Bind<IExceptionFilter>().To<ExceptionFilter>();

        return ninjectKernel;
    }

    public static StandardKernel WithApiClients(this StandardKernel ninjectKernel)
    {
        var settings = ninjectKernel.Get<IAppSettingsService>().GetSettings();
        var restClient = RestClientBuilder.BuildRestClient(settings.ApiUrl, false);
        ninjectKernel.Bind<RestSharp.RestClient>().ToConstant(restClient);
        ninjectKernel.Bind<IApiClient>().To<ApiClient>();

        return ninjectKernel;
    }

    public static StandardKernel WithDiscordClient(this StandardKernel ninjectKernel)
    {
        var settings = ninjectKernel.Get<IAppSettingsService>().GetSettings();
        var discordClient = new DiscordClient(new DiscordConfiguration
        {
            Token = settings.DiscordToken,
            TokenType = TokenType.Bot,
            MinimumLogLevel = Enum.TryParse<LogLevel>(settings.LogLevel, out var level) ? level : LogLevel.Trace,
            Intents = DiscordIntents.All
        });
        ninjectKernel.Bind<DiscordClient>().ToConstant(discordClient);

        return ninjectKernel;
    }

    public static StandardKernel WithDiscordWrapper(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IDiscordClientWrapper>().To<DiscordClientWrapper.DiscordClientWrapper>();

        return ninjectKernel;
    }

    public static StandardKernel WithEmoteStatisticService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IEmoteStatsService>().To<EmoteStatsService>();

        return ninjectKernel;
    }

    public static StandardKernel WithDailyStatisticService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IDailyStatisticsService>().To<DailyStatisticsService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithCustomUserBalanceService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IUserBalanceService>().To<UserBalanceService>();

        return ninjectKernel;
    }

    public static StandardKernel WithRandomizer(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IRandomizer>().To<Randomizer>();

        return ninjectKernel;
    }

    public static StandardKernel WithPartyService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IPartyService>().To<PartyService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithRaceService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IRaceService>().To<RaceService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithLotteryService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<ILotteryService>().To<LotteryService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithGuessNumberService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IGuessNumberService>().To<GuessNumberService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithLohotron(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<Lohotron>().ToSelf().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithInventoryService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IUserInventoryService>().To<UserInventoryService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithShopService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IShopService>().To<ShopService>().InSingletonScope();

        return ninjectKernel;
    }

    public static StandardKernel WithTributeService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<TributeService>().ToSelf();

        return ninjectKernel;
    }

    public static StandardKernel WithEvents(this StandardKernel ninjectKernel)
    {
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IEvent).IsAssignableFrom(p))
            .Where(p => p != typeof(IEvent));

        foreach (var eventType in eventTypes)
        {
            ninjectKernel.Bind(eventType).ToSelf();
            ninjectKernel.Bind<IEvent>().To(eventType);
        }

        return ninjectKernel;
    }

    public static StandardKernel WithNightEvents(this StandardKernel ninjectKernel)
    {
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(INightEvent).IsAssignableFrom(p))
            .Where(p => p != typeof(INightEvent));

        foreach (var eventType in eventTypes)
        {
            ninjectKernel.Bind(eventType).ToSelf();
            ninjectKernel.Bind<INightEvent>().To(eventType);
        }

        return ninjectKernel;
    }

    public static StandardKernel WithEventScheduler(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<EventScheduler>().ToSelf();

        return ninjectKernel;
    }

    public static StandardKernel WithDailyEvents(this StandardKernel ninjectKernel)
    {
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IDailyEvent).IsAssignableFrom(p))
            .Where(p => p != typeof(IDailyEvent));

        foreach (var eventType in eventTypes)
        {
            ninjectKernel.Bind(eventType).ToSelf();
            ninjectKernel.Bind<IDailyEvent>().To(eventType);
        }

        return ninjectKernel;
    }

    public static StandardKernel WithDailyEventScheduler(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<DailyEventScheduler>().ToSelf();

        return ninjectKernel;
    }

    public static StandardKernel WithCommands(this StandardKernel ninjectKernel)
    {
        var commandTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ICommand).IsAssignableFrom(p))
            .Where(p => p != typeof(ICommand))
            .ToList();

        foreach (var commandType in commandTypes)
        {
            ninjectKernel.Bind(commandType).ToSelf();
            ninjectKernel.Bind<ICommand>().To(commandType);
        }

        return ninjectKernel;
    }

    public static StandardKernel WithCommandsService(this StandardKernel ninjectKernel)
    {
        var commandsService = new CommandsService(
            ninjectKernel.Get<IDiscordClientWrapper>(),
            ninjectKernel.Get<IAppSettingsService>(),
            ninjectKernel.Get<IGuildSettingsService>()
        );

        ninjectKernel.Bind<ICommandsService>().ToConstant(commandsService);

        return ninjectKernel;
    }

    public static StandardKernel ConfigureCommands(this StandardKernel ninjectKernel)
    {
        var commandsService = ninjectKernel.Get<ICommandsService>();
        
        var commands = ninjectKernel.GetAll<ICommand>();
        var commandsByName = commands.ToDictionary(command => command.Name);
        
        commandsService.UseCommands(commandsByName);

        return ninjectKernel;
    }

    public static StandardKernel WithApiEventFeedConsumer(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IApiEventFeedConsumer>().To<ApiEventFeedConsumer>();

        return ninjectKernel;
    }

    public static StandardKernel WithServicesHealthChecker(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IServicesHealthChecker>().To<ServicesHealthChecker>();

        return ninjectKernel;
    }

    public static StandardKernel WithMinecraftServerInfoScheduler(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IMinecraftServerInfoScheduler>().To<MinecraftServerInfoScheduler>();

        return ninjectKernel;
    }

    public static StandardKernel WithDiscordBotBehaviour(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IDiscordBotBehaviour>().To<DiscordBotBehaviour>();

        return ninjectKernel;
    }

    public static StandardKernel WithIpService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IIpService>().To<IpService>();

        return ninjectKernel;
    }

    public static StandardKernel WithMinecraftServerInfoService(this StandardKernel ninjectKernel)
    {
        ninjectKernel.Bind<IMinecraftServerInfoService>().To<MinecraftServerInfoService>();

        return ninjectKernel;
    }
}