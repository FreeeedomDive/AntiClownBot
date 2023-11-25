using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1PredictionsStats;

public class F1PredictionStatsEmbedBuilder : IF1PredictionStatsEmbedBuilder
{
    public DiscordEmbed Build(MostPickedDriversStatsDto mostPickedDriversStats)
    {
        var embedBuilder = new DiscordEmbedBuilder().WithTitle("Самые выбираемые гонщики");
        AddInlineFieldWithRating(embedBuilder, "10 место", mostPickedDriversStats.TenthPlacePickedDrivers);
        AddInlineFieldWithRating(embedBuilder, "Первый DNF", mostPickedDriversStats.FirstDnfPickedDrivers);

        return embedBuilder.Build();
    }

    public DiscordEmbed Build(MostProfitableDriversStatsDto mostProfitableDriversStats)
    {
        var embedBuilder = new DiscordEmbedBuilder().WithTitle("\"Правильные ответы\" на предсказания");
        AddInlineFieldWithRating(embedBuilder, "Очки за 10 место", mostProfitableDriversStats.TenthPlacePoints);
        AddInlineFieldWithRating(embedBuilder, "10 место", mostProfitableDriversStats.TenthPlaceCount);
        AddInlineFieldWithRating(embedBuilder, "Первый DNF", mostProfitableDriversStats.FirstDnfCount);

        return embedBuilder.Build();
    }

    private static void AddInlineFieldWithRating(DiscordEmbedBuilder embedBuilder, string title, DriverStatisticsDto[] drivers)
    {
        embedBuilder.AddField(
            title,
            string.Join(
                "\n",
                drivers.Select(
                    (driver, index) => $"{index + 1}. {driver.Driver}: {driver.Score}"
                )
            ),
            true
        );
    }
}