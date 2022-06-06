using AntiClownDiscordBotVersion2.ApiPoll;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Events;
using Ninject;

namespace AntiClownDiscordBotVersion2;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configurator = new DependenciesConfigurator.DependenciesConfigurator().BuildDependencies();
        Console.WriteLine("Configured all dependencies");
        StartBackgroundApiPollScheduler(configurator);
        Console.WriteLine("Started API poll scheduler");
        StartBackgroundDailyEventScheduler(configurator);
        Console.WriteLine("Started DailyEvent scheduler");
        StartBackgroundEventScheduler(configurator);
        Console.WriteLine("Started Event scheduler");
        Console.WriteLine("Start listening to discord events...");
        await StartDiscordAsync(configurator);
    }

    private static void StartBackgroundApiPollScheduler(StandardKernel configurator)
    {
        var apiPollingScheduler = configurator.Get<IApiPollScheduler>();
        apiPollingScheduler.Start();
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

    private static async Task StartDiscordAsync(StandardKernel configurator)
    {
        var discordClient = configurator.Get<IDiscordClientWrapper>();
        await discordClient.StartDiscord();
    }
}