using System.Globalization;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public class F1PredictionsEmbedBuilder : IF1PredictionsEmbedBuilder
{
    public F1PredictionsEmbedBuilder(IUsersCache usersCache)
    {
        this.usersCache = usersCache;
    }

    public async Task<DiscordEmbed> BuildPredictionUpdatedAsync(F1UserPredictionUpdatedMessageDto message, F1RaceDto race, F1PredictionDto prediction)
    {
        var member = await usersCache.GetMemberByApiIdAsync(prediction.UserId);
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle($"{member.ServerOrUserName()} {(message.IsNew ? "добавил" : "обновил")} свое предсказание на гонку {race.Name} {race.Season}")
                           .WithColor(DiscordColor.Gold);
        return embedBuilder.Build();
    }

    public DiscordEmbed BuildResultsUpdated(F1RaceDto race)
    {
        return new DiscordEmbedBuilder()
               .WithTitle($"Внесены результаты для гонки {race.Name} {race.Season}")
               .WithColor(DiscordColor.DarkGreen)
               .AddField(
                   "Классификация",
                   string.Join(
                       "\n",
                       race.Result.Classification.Batch(10).Select(x => string.Join(" ", x.Select(driver => driver.Trigram())))
                   )
               )
               .AddField("DNF", string.Join(" ", race.Result.DnfDrivers.Select(driver => driver.Trigram())))
               .AddField("Количество инцидентов", race.Result.SafetyCars.ToString())
               .AddField("Отрыв лидера", race.Result.FirstPlaceLead.ToString(CultureInfo.InvariantCulture))
               .Build();
    }

    public DiscordEmbed BuildRaceFinished(F1PredictionUserResultDto[] results)
    {
        var apiIdToMember = results.ToDictionary(
            x => x.UserId,
            x => usersCache.GetMemberByApiIdAsync(x.UserId).GetAwaiter().GetResult()
        );
        var embedBuilder = new DiscordEmbedBuilder()
            .WithTitle("Результаты предсказаний");
        results.ForEach(
            x =>
            {
                embedBuilder.AddField(
                    apiIdToMember[x.UserId].ServerOrUserName(),
                    $"{x.TenthPlacePoints.ToPluralizedString("очко", "очка", "очков")} за 10 место\n"
                    + $"{x.DnfsPoints.ToPluralizedString("очко", "очка", "очков")} за DNF\n"
                    + $"{x.SafetyCarsPoints.ToPluralizedString("очко", "очка", "очков")} за SC\n"
                    + $"{x.FirstPlaceLeadPoints.ToPluralizedString("очко", "очка", "очков")} за отрыв лидера\n"
                    + $"{x.TeamMatesPoints.ToPluralizedString("очко", "очка", "очков")} за победителей внутри команд\n"
                );
            }
        );
        return embedBuilder.Build();
    }

    private readonly IUsersCache usersCache;
}