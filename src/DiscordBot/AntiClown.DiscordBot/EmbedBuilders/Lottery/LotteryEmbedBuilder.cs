using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Lottery;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Lottery;

public class LotteryEmbedBuilder : ILotteryEmbedBuilder
{
    public LotteryEmbedBuilder(
        IUsersCache usersCache,
        IEmotesCache emotesCache,
        IAntiClownDataApiClient antiClownDataApiClient
    )
    {
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    public async Task<DiscordEmbed> BuildAsync(LotteryEventDto lottery)
    {
        var embedBuilder = new DiscordEmbedBuilder()
                           .WithTitle("Лотерея")
                           .WithColor(DiscordColor.Magenta)
                           .WithFooter($"EventId: {lottery.Id}");
        if (lottery.Finished)
        {
            if (lottery.Participants.Count == 0)
            {
                var sadge = await emotesCache.GetEmoteAsTextAsync("Sadge");
                embedBuilder.AddField("Никто не принял участие", $"{sadge}");
            }
            else
            {
                foreach (var participant in lottery.Participants.Values)
                {
                    var member = await usersCache.GetMemberByApiIdAsync(participant.UserId);
                    var slotsTasks = participant.Slots.Select(x => emotesCache.GetEmoteAsTextAsync(x.ToEmoteName()));
                    var emotes = await Task.WhenAll(slotsTasks);
                    embedBuilder.AddField(
                        $"{member.ServerOrUserName()} - {participant.Prize}",
                        string.Join(" ", emotes)
                    );
                }
            }
        }
        else
        {
            var emotesInfo = await Task.WhenAll(
                Enum
                    .GetValues<LotterySlotDto>()
                    .Select(async x => $"{await emotesCache.GetEmoteAsTextAsync(x.ToEmoteName())}: {x.ToPrize()}"));
            var waitingTime = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.CommonEvents, "LotteryEvent.WaitingTimeInMilliseconds");
            embedBuilder.AddField("Начинаем лотерею!", $"Здесь можно выиграть много скам-койнов!\nПодведем итоги через {waitingTime / (60 * 1000)} минут")
                        .AddField("Эмоты, которые могут выпасть:", string.Join("\n", emotesInfo))
                        .AddField("Комбинации:", "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 и более = x100");
        }

        return embedBuilder.Build();
    }

    private readonly IEmotesCache emotesCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IUsersCache usersCache;
}