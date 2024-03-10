using System.Globalization;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Messages.Dto.F1Predictions;
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
                           .WithTitle($"{member.ServerOrUserName()} {(message.IsNew ? "добавил" : "обновил")} свое предсказание на гонку {race.Name}")
                           .WithColor(DiscordColor.Gold)
                           .AddField("10 место", prediction.TenthPlacePickedDriver.ToString())
                           .AddField("DNF", prediction.DnfPrediction.NoDnfPredicted ? "Никто" : string.Join(" ", prediction.DnfPrediction.DnfPickedDrivers!))
                           .AddField("Количество машин безопасности", prediction.SafetyCarsPrediction.ToNumberedString())
                           .AddField("Отрыв 1 места", prediction.FirstPlaceLeadPrediction.ToString(CultureInfo.InvariantCulture))
                           .AddField("Кто из команды окажется впереди", string.Join(" ", prediction.TeamsPickedDrivers));
        return embedBuilder.Build();
    }

    public DiscordEmbed BuildPredictionsList(F1RaceDto race)
    {
        var apiIdToMember = race.Predictions.ToDictionary(
            x => x.UserId,
            x => usersCache.GetMemberByApiIdAsync(x.UserId).GetAwaiter().GetResult()
        );
        return new DiscordEmbedBuilder()
               .WithTitle($"Предсказания на гонку {race.Name} {race.Season}")
               .AddField(
                   "Предсказатель",
                   string.Join(
                       "\n",
                       race.Predictions.Select(p => apiIdToMember[p.UserId].ServerOrUserName())
                   ), true
               )
               .AddField(
                   "10 место",
                   string.Join(
                       "\n",
                       race.Predictions.Select(p => p.TenthPlacePickedDriver.ToString())
                   ), true
               )
               .AddField(
                   "DNF",
                   string.Join(
                       "\n",
                       race.Predictions.Select(p => p.DnfPrediction.NoDnfPredicted ? "Никто" : string.Join(" ", p.DnfPrediction.DnfPickedDrivers!))
                   ), true
               )
               .AddField(
                   "SC",
                   string.Join(
                       "\n",
                       race.Predictions.Select(p => p.SafetyCarsPrediction.ToNumberedString())
                   ), true
               )
               .AddField(
                   "Отрыв лидера",
                   string.Join(
                       "\n",
                       race.Predictions.Select(p => p.FirstPlaceLeadPrediction.ToString(CultureInfo.InvariantCulture))
                   ), true
               ).Build();
    }

    private readonly IUsersCache usersCache;
}