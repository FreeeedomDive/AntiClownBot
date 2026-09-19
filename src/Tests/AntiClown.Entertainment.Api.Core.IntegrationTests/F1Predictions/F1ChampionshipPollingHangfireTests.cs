using AntiClown.Core.Schedules;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.ExternalClients.Jolpica;
using AntiClown.Entertainment.Api.Core.F1Predictions.Options;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services;
using AntiClown.Entertainment.Api.Core.F1Predictions.Services.ChampionshipPredictions;
using AntiClown.Tests.Configuration;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;

namespace AntiClown.Entertainment.Api.Core.IntegrationTests.F1Predictions;

[NonParallelizable]
public class F1ChampionshipPollingHangfireTests
    : IntegrationTestsBase<F1HangfireIntegrationTestsWebApplicationFactory, Program>
{
    [Test]
    public async Task FinishRaceAsync_Should_RetryChampionshipPollingAfterTransientFailure()
    {
        var service = Scope.ServiceProvider.GetRequiredService<IF1PredictionsService>();
        var championshipService = Scope.ServiceProvider.GetRequiredService<IF1ChampionshipPredictionsService>();
        var jolpicaClient = Scope.ServiceProvider.GetRequiredService<IJolpicaClient>();
        var timeProvider = Scope.ServiceProvider.GetRequiredService<TimeProvider>();
        timeProvider.GetUtcNow().Returns(new DateTimeOffset(2026, 8, 3, 0, 0, 0, TimeSpan.Zero));

        await service.CreateOrUpdateTeamAsync(new F1Team("Test Team", "Driver1", "Driver2"));
        jolpicaClient.GetQualifyingDriverNamesAsync(2026, 1).Returns(["Driver1", "Driver2"]);
        jolpicaClient.GetDriverStandingsAsync(2026).Returns(
            _ => Task.FromException<(int Round, string[] Standings)?>(
                new HttpRequestException("Jolpica is temporarily unavailable")
            ),
            _ => Task.FromResult<(int Round, string[] Standings)?>((1, ["Driver1", "Driver2"]))
        );

        var raceId = await service.StartNewRaceAsync("Hangfire Test GP", false);
        await service.AddRaceResultAsync(raceId, new F1PredictionRaceResult
        {
            RaceId = raceId,
            Classification = ["Driver1", "Driver2"],
            DnfDrivers = [],
            SafetyCars = 0,
            FirstPlaceLead = 1,
        });

        await service.FinishRaceAsync(raceId);

        F1ChampionshipResults? results = null;
        for (var attempt = 0; attempt < 100; attempt++)
        {
            results = await championshipService.ReadResultsAsync(2026);
            if (results.HasData)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(100));
        }

        results.Should().NotBeNull();
        results!.HasData.Should().BeTrue();
        results.Standings.Should().Equal("Driver1", "Driver2");
        await jolpicaClient.Received(2).GetDriverStandingsAsync(2026);
    }
}

public class F1HangfireIntegrationTestsWebApplicationFactory
    : EntertainmentApiIntegrationTestsWebApplicationFactory
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        base.ConfigureTestServices(services);
        services.RemoveAll<IScheduler>();
        services.AddTransient<IScheduler, HangfireScheduler>();
        services.PostConfigure<F1PredictionsOptions>(options =>
            options.ChampionshipPollingInterval = TimeSpan.FromMilliseconds(100)
        );
        services.AddHangfireServer(options =>
            options.SchedulePollingInterval = TimeSpan.FromMilliseconds(100)
        );
    }
}
