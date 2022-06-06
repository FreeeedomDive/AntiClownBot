using Ninject;

namespace AntiClownDiscordBotVersion2.DependenciesConfigurator;

public class DependenciesConfigurator
{
    public StandardKernel BuildDependencies()
    {
        var ninjectKernel = new StandardKernel();
        ninjectKernel
            // configure settings
            .WithApplicationSettings()
            .WithEventsSettings()
            .WithGuildSettings()
            // configure logger
            .WithLogger()
            // configure other stuff without dependencies
            .WithRandomizer()
            // configure api clients
            .WithApiClients()
            .WithDiscordClient()
            .WithDiscordWrapper()
            // configure statistic service
            .WithEmoteStatisticService()
            .WithDailyStatisticService()
            // configure custom user balance service
            .WithCustomUserBalanceService()
            // configure services and event-related things
            .WithPartyService()
            .WithRaceService()
            .WithLotteryService()
            .WithGuessNumberService()
            .WithLohotron()
            .WithInventoryService()
            .WithShopService()
            .WithTributeService()
            // configure events and service
            .WithEvents()
            .WithEventScheduler()
            .WithDailyEvents()
            .WithDailyEventScheduler()
            // configure commands and service
            .WithCommandsService()
            .WithCommands()
            .ConfigureCommands()
            // final touches
            .WithApiPollerScheduler()
            .WithDiscordBotBehaviour();

        return ninjectKernel;
    }
}