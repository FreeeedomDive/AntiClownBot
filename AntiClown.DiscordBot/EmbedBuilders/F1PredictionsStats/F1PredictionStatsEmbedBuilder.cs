using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1PredictionsStats;

public class F1PredictionStatsEmbedBuilder : IF1PredictionStatsEmbedBuilder
{
    public DiscordEmbed Build(MostPickedDriversByUsersStatsDto mostPickedDriversByUsersStats)
    {
        var embedBuilder = new DiscordEmbedBuilder().WithTitle("Самые выбираемые гонщики");
        AddInlineFieldWithRating(embedBuilder, "10 место", mostPickedDriversByUsersStats.TenthPlacePickedDrivers);
        AddInlineFieldWithRating(embedBuilder, "Первый DNF", mostPickedDriversByUsersStats.FirstDnfPickedDrivers);

        return embedBuilder.Build();
    }

    private static void AddInlineFieldWithRating(DiscordEmbedBuilder embedBuilder, string title, DriverStatisticsDto[] drivers)
    {
        var longestNameLength = drivers.Select(x => x.Driver.ToString().Length).Max();
        embedBuilder.AddField(
            "Первый DNF",
            string.Join(
                "\n",
                drivers.Select(
                    (driver, index) => $"{index + 1}. {driver.Driver}:{" ".Repeat(longestNameLength - driver.Driver.ToString().Length + 1)}{driver.Score}"
                )
            ),
            true
        );
    }
}