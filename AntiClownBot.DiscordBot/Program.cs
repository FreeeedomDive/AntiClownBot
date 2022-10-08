using AntiClownDiscordBotVersion2.ApiEventFeed;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;
using AntiClownDiscordBotVersion2.Events;
using AntiClownDiscordBotVersion2.ExceptionFilters;
using AntiClownDiscordBotVersion2.MinecraftServer;
using AntiClownDiscordBotVersion2.ServicesHealth;
using Loggers;
using Ninject;

namespace AntiClownDiscordBotVersion2;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configurator = new Dependencies.DependenciesConfigurator().BuildDependencies();
        Console.WriteLine("Configured all dependencies");
        AddExceptionLogger(configurator);
        StartBackgroundApiPollScheduler(configurator);
        StartBackgroundServiceChecker(configurator);
        Console.WriteLine("Started API poll scheduler");
        StartBackgroundDailyEventScheduler(configurator);
        Console.WriteLine("Started DailyEvent scheduler");
        StartBackgroundEventScheduler(configurator);
        Console.WriteLine("Started Event scheduler");
        StartBackgroundMinecraftServerInfoScheduler(configurator);
        Console.WriteLine("Started MinecraftServerInfo scheduler");
        Console.WriteLine("Start listening to discord events...");
        await StartDiscordAsync(configurator);
    }

    private static void AddExceptionLogger(StandardKernel configurator)
    {
        var logger = configurator.Get<ILogger>();
        var exceptionFilter = configurator.Get<IExceptionFilter>();
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
        {
            if (exceptionFilter.Filter(eventArgs.Exception))
            {
                return;
            }
            Task.Run(() => logger.Error(eventArgs.Exception, "Unhandled exception in DiscordBot"));
        };
    }

    private static void StartBackgroundApiPollScheduler(StandardKernel configurator)
    {
        var apiPollingScheduler = configurator.Get<IApiEventFeedConsumer>();
        apiPollingScheduler.Start();
    }

    private static void StartBackgroundServiceChecker(StandardKernel configurator)
    {
        var servicesHealthChecker = configurator.Get<IServicesHealthChecker>();
        servicesHealthChecker.Start();
    }

    private static void StartBackgroundDailyEventScheduler(StandardKernel configurator)
    {
        var dailyEventScheduler = configurator.Get<DailyEventScheduler>();
        dailyEventScheduler.Start();
    }

    private static void StartBackgroundEventScheduler(StandardKernel configurator)
    {
        var eventScheduler = configurator.Get<EventScheduler>();
        eventScheduler.Start();
    }

    private static void StartBackgroundMinecraftServerInfoScheduler(StandardKernel configurator)
    {
        var eventScheduler = configurator.Get<IMinecraftServerInfoScheduler>();
        eventScheduler.Start();
    }

    private static async Task StartDiscordAsync(StandardKernel configurator)
    {
        var discordBehaviourConfigurator = configurator.Get<IDiscordBotBehaviour>();
        discordBehaviourConfigurator.Configure();
        var discordClient = configurator.Get<IDiscordClientWrapper>();
        await discordClient.StartDiscord();
    }
}