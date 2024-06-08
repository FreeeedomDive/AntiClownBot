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
                           .WithTitle($"{member.ServerOrUserName()} {(message.IsNew ? "добавил" : "обновил")} свое предсказание на гонку {race.Name} {race.Season}");
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
               .AddField("DNF", race.Result.DnfDrivers.Length == 0 ? "Никто" : string.Join(" ", race.Result.DnfDrivers.Select(driver => driver.Trigram())))
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
                           .WithTitle($"Результаты предсказаний на гонку {race.Name} {race.Season}");
        results.ForEach(
            userResult =>
            {
                var userPrediction = race.Predictions.First(x => x.UserId == userResult.UserId);
                embedBuilder.AddField(
                    apiIdToMember[userResult.UserId].ServerOrUserName(),
                    $"10 место: {userPrediction.TenthPlacePickedDriver.Trigram()} - "
                    + $"{userResult.TenthPlacePoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"DNF: {(userPrediction.DnfPrediction.NoDnfPredicted ? "Никто" : string.Join(" ", userPrediction.DnfPrediction.DnfPickedDrivers!.Select(x => x.Trigram())))} - "
                    + $"{userResult.DnfsPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Инциденты: {userPrediction.SafetyCarsPrediction} - "
                    + $"{userResult.SafetyCarsPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Отрыв лидера: {userPrediction.FirstPlaceLeadPrediction} - "
                    + $"{userResult.FirstPlaceLeadPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                    + $"Победители внутри команд: {string.Join(" ", userPrediction.TeamsPickedDrivers.Select(x => x.Trigram()))} - "
                    + $"{userResult.TeamMatesPoints.ToPluralizedString("очко", "очка", "очков")}\n"
                );
            }
        );
        return embedBuilder.Build();
    }

    private readonly IUsersCache usersCache;
}