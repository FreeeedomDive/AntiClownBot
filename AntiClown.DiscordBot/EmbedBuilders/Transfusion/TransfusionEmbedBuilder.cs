using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using AntiClown.Entertainment.Api.Dto.CommonEvents.Transfusion;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Transfusion;

public class TransfusionEmbedBuilder : ITransfusionEmbedBuilder
{
    public TransfusionEmbedBuilder(
        IUsersCache usersCache,
        IEmotesCache emotesCache
    )
    {
        this.usersCache = usersCache;
        this.emotesCache = emotesCache;
    }

    public async Task<DiscordEmbed> BuildAsync(TransfusionEventDto transfusionEvent)
    {
        var donorMember = await usersCache.GetMemberByApiIdAsync(transfusionEvent.DonorUserId);
        var recipientMember = await usersCache.GetMemberByApiIdAsync(transfusionEvent.RecipientUserId);
        return new DiscordEmbedBuilder()
               .WithTitle("Перекачка скам-койнов")
               .WithColor(DiscordColor.Chartreuse)
               .AddField(
                   "Я решил немного перемешать экономику,\n" +
                   "поэтому возьму немного скам-койнов у " +
                   $"{donorMember.ServerOrUserName()}.",
                   $"Отдай {recipientMember.ServerOrUserName()} "
                   + $"{transfusionEvent.Exchange.ToPluralizedString("скам-койн", "скам-койна", "скам-койнов")} "
                   + $"{await emotesCache.GetEmoteAsTextAsync("Okayge")}"
               )
               .WithFooter($"EventId: {transfusionEvent.Id}")
               .Build();
    }

    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}