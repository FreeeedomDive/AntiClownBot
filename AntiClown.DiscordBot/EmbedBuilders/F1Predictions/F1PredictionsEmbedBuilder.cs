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

    public async Task<DiscordEmbed> BuildAsync(F1UserPredictionUpdatedMessageDto message, F1RaceDto race, F1PredictionDto prediction)
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

    private readonly IUsersCache usersCache;
}