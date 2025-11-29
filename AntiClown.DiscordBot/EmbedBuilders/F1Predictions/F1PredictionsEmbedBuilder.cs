using System.Globalization;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Options;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
using AntiClown.Tools.Utility.Extensions;
using DSharpPlus.Entities;
using Microsoft.Extensions.Options;

namespace AntiClown.DiscordBot.EmbedBuilders.F1Predictions;

public class F1PredictionsEmbedBuilder(
    IUsersCache usersCache,
    IOptions<WebOptions> webOptions
) : IF1PredictionsEmbedBuilder
{
    public DiscordEmbed BuildPredictionStarted(string raceName)
    {
        return new DiscordEmbedBuilder()
               .WithColor(DiscordColor.Violet)
               .AddField("Открыто новое предсказание", $"Начались сборы предсказаний на гонку {raceName}")
               .Build();
    }

    public async Task<DiscordEmbed> BuildPredictionUpdatedAsync(F1UserPredictionUpdatedMessageDto message, F1RaceDto race, F1PredictionDto prediction)
    {
        var member = await usersCache.GetMemberByApiIdAsync(prediction.UserId);
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithColor(DiscordColor.MidnightBlue)
                           .WithTitle($"{member.ServerOrUserName()} {(message.IsNew ? "добавил" : "обновил")} свое предсказание на гонку {race.FullName()} {race.Season}");
        return embedBuilder.Build();
    }

    public DiscordEmbed BuildResultsUpdated(F1RaceDto race)
    {
        return new DiscordEmbedBuilder()
               .WithTitle($"Внесены результаты для гонки {race.FullName()} {race.Season}")
               .WithColor(DiscordColor.DarkGreen)
               .AddField(
                   "Классификация",
                   string.Join(
                       "\n",
                       race.Result.Classification.Batch(10).Select(x => string.Join(" ", x.Select(Trigram)))
                   )
               )
               .AddField("DNF", race.Result.DnfDrivers.Length == 0 ? "Никто" : string.Join(" ", race.Result.DnfDrivers.Select(Trigram)))
               .AddField("Количество инцидентов", race.Result.SafetyCars.ToString())
               .AddField("Отрыв лидера", race.Result.FirstPlaceLead.ToString(CultureInfo.InvariantCulture))
               .Build();
    }

    public DiscordEmbed BuildRaceFinished(F1RaceDto race, F1PredictionUserResultDto[] results)
    {
        var apiIdToMember = results.ToDictionary(
            x => x.UserId,
            x => usersCache.GetMemberByApiIdAsync(x.UserId).GetAwaiter().GetResult()
        );
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithColor(DiscordColor.Gold)
                           .WithTitle($"Результаты предсказаний на гонку {race.FullName()} {race.Season}");
        results.ForEach(
            userResult =>
            {
                var userPrediction = race.Predictions.First(x => x.UserId == userResult.UserId);
                embedBuilder.AddField(
                    apiIdToMember[userResult.UserId].ServerOrUserName(),
                    $"10 место: {Trigram(userPrediction.TenthPlacePickedDriver)} - "
                    + $"{userResult.TenthPlacePoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"DNF: {(userPrediction.DnfPrediction.NoDnfPredicted ? "Никто" : string.Join(" ", userPrediction.DnfPrediction.DnfPickedDrivers!.Select(Trigram)))} - "
                    + $"{userResult.DnfsPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Инциденты: {userPrediction.SafetyCarsPrediction} - "
                    + $"{userResult.SafetyCarsPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Отрыв лидера: {userPrediction.FirstPlaceLeadPrediction} - "
                    + $"{userResult.FirstPlaceLeadPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Победители внутри команд: {string.Join(" ", userPrediction.TeamsPickedDrivers.Select(Trigram))} - "
                    + $"{userResult.TeamMatesPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                );
            }
        );
        return embedBuilder.Build();
    }

    public async Task<DiscordEmbed> BuildBingoCompletedAsync(Guid userId)
    {
        var member = await usersCache.GetMemberByApiIdAsync(userId);
        return new DiscordEmbedBuilder()
               .WithTitle("Бинго!")
               .WithColor(DiscordColor.Orange)
               .AddField(
                   $"У {member.ServerOrUserName()} бинго!",
                   $"{webOptions.Value.FrontApplicationUrl}/user/{userId}/f1Predictions/bingo"
               )
               .Build();
    }

    private static string Trigram(string x)
    {
        return x[..3].ToUpper();
    }
}