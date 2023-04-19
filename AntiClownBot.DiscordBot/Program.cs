using AntiClownDiscordBotVersion2.ApiEventFeed;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;
using AntiClownDiscordBotVersion2.Events;
using AntiClownDiscordBotVersion2.ExceptionFilters;
using AntiClownDiscordBotVersion2.MinecraftServer;
using AntiClownDiscordBotVersion2.ServicesHealth;
using Ninject;
using TelemetryApp.Api.Client.Log;

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
        // StartBackgroundMinecraftServerInfoScheduler(configurator);
        // Console.WriteLine("Started MinecraftServerInfo scheduler");
        // StartTimeOffsetTestTask(configurator);
        Console.WriteLine("Start listening to discord events...");
        await StartDiscordAsync(configurator);
    }

    private static void AddExceptionLogger(StandardKernel configurator)
    {
        var logger = configurator.Get<ILoggerClient>();
        var exceptionFilter = configurator.Get<IExceptionFilter>();
        AppDomain.CurrentDomain.FirstChanceException += (_, eventArgs) =>
        {
            if (exceptionFilter.Filter(eventArgs.Exception))
            {
                return;
            }

            Console.WriteLine(eventArgs.Exception.Message);
            Console.WriteLine(eventArgs.Exception.StackTrace);
            logger.ErrorAsync(eventArgs.Exception, "Unhandled exception in DiscordBot").GetAwaiter().GetResult();
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

    private static void StartTimeOffsetTestTask(StandardKernel configurator)
    {
        var test = configurator.Get<TimeOffsetTestTask>();
        test.Start();
    }

    private static async Task StartDiscordAsync(StandardKernel configurator)
    {
        var discordBehaviourConfigurator = configurator.Get<IDiscordBotBehaviour>();
        await discordBehaviourConfigurator.ConfigureAsync();
        var discordClient = configurator.Get<IDiscordClientWrapper>();
        await discordClient.StartDiscord();
    }
}