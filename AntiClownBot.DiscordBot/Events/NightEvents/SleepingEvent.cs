using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;

namespace AntiClownDiscordBotVersion2.Events.NightEvents;

public class SleepingEvent : INightEvent
{
    public SleepingEvent(
        IDiscordClientWrapper discordClientWrapper,
        IEmotesProvider emotesProvider,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesProvider = emotesProvider;
        this.guildSettingsService = guildSettingsService;
    }
    
    public async Task ExecuteAsync()
    {
        var bedgeEmote = await emotesProvider.GetEmoteAsTextAsync("Bedge");
        await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, bedgeEmote);
    }
    
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesProvider emotesProvider;
    private readonly IGuildSettingsService guildSettingsService;
}