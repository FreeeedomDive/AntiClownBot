using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.Entertainment.Api.Dto.CommonEvents.RemoveCoolDowns;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.RemoveCoolDowns;

public class RemoveCoolDownsEmbedBuilder : IRemoveCoolDownsEmbedBuilder
{
    public RemoveCoolDownsEmbedBuilder(
        IEmotesCache emotesCache
    )
    {
        this.emotesCache = emotesCache;
    }

    public async Task<DiscordEmbed> BuildAsync(RemoveCoolDownsEventDto removeCoolDownsEvent)
    {
        return new DiscordEmbedBuilder()
               .WithTitle("Сброс перезарядки подношений")
               .WithColor(DiscordColor.Blurple)
               .AddField("У меня хорошее настроение!", $"Несите ваши подношения {await emotesCache.GetEmoteAsTextAsync("peepoClap")}")
               .WithFooter($"EventId: {removeCoolDownsEvent.Id}")
               .Build();
    }

    private readonly IEmotesCache emotesCache;
}