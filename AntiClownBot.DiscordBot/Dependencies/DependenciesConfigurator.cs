using Ninject;
using TelemetryApp.Utilities.Extensions;

namespace AntiClownDiscordBotVersion2.Dependencies;

public class DependenciesConfigurator
{
    public StandardKernel BuildDependencies()
    {
        var ninjectKernel = new StandardKernel();
        ninjectKernel
            .WithDiKernel()
            // configure settings
            .WithApplicationSettings()
            .WithEventsSettings()
            .WithGuildSettings()
            // configure telemetry api clients
            .WithTelemetryClientWithLogger()
            // configure other stuff without dependencies
            .WithRandomizer()
            .WithIpService()
            .WithMinecraftServerInfoService()
            .WithTimeOffsetTestTask()
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
            .WithF1PredictionsService()
            // configure events and service
            .WithEvents()
            .WithNightEvents()
            .WithEventScheduler()
            .WithDailyEvents()
            .WithDailyEventScheduler()
            // configure commands and service
            .WithCommandsService()
            .WithCommands()
            .ConfigureCommands()
            // final touches
            .WithExceptionFilter()
            .WithApiEventFeedConsumer()
            .WithServicesHealthChecker()
            .WithMinecraftServerInfoScheduler()
            .WithDiscordBotBehaviour();

        return ninjectKernel;
    }
}