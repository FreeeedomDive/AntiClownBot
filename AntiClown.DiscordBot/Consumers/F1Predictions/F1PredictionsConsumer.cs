using System.Globalization;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Messages.Dto.F1Predictions;
using DSharpPlus.Entities;
using MassTransit;

namespace AntiClown.DiscordBot.Consumers.F1Predictions;

public class F1PredictionsConsumer : IConsumer<F1UserPredictionUpdatedMessageDto>
{
    public F1PredictionsConsumer(
        IAntiClownDataApiClient antiClownDataApiClient,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IDiscordClientWrapper discordClientWrapper,
        IUsersCache usersCache
    )
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.discordClientWrapper = discordClientWrapper;
        this.usersCache = usersCache;
    }

    public async Task Consume(ConsumeContext<F1UserPredictionUpdatedMessageDto> context)
    {
        var message = context.Message;
        var race = await antiClownEntertainmentApiClient.F1Predictions.ReadAsync(message.RaceId);
        var prediction = race.Predictions.FirstOrDefault(x => x.UserId == message.UserId);
        if (prediction is null)
        {
            return;
        }
        var member = await usersCache.GetMemberByApiIdAsync(message.UserId);
        var embedBuilder = new DiscordEmbedBuilder()
            .WithTitle($"{member.ServerOrUserName()} обновил свое предсказание на гонку {race.Name}")
            .WithColor(DiscordColor.Gold)
            .AddField("10 место", prediction.TenthPlacePickedDriver.ToString())
            .AddField("DNF", prediction.DnfPrediction.NoDnfPredicted ? "Никто" : string.Join(" ", prediction.DnfPrediction.DnfPickedDrivers!))
            .AddField("Количество машин безопасности", prediction.SafetyCarsPrediction.ToString())
            .AddField("Отрыв 1 места", prediction.FirstPlaceLeadPrediction.ToString(CultureInfo.InvariantCulture))
            .AddField("Кто из команды окажется впереди", string.Join(" ", prediction.TeamsPickedDrivers));
        var f1PredictionsChatId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1PredictionsChatId");
        await discordClientWrapper.Messages.SendAsync(f1PredictionsChatId, embedBuilder.Build());
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IUsersCache usersCache;
}