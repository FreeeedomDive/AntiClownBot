using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;

namespace AntiClownDiscordBotVersion2.Events.NightEvents;

public class SleepingEvent : INightEvent
{
    public SleepingEvent(
        IDiscordClientWrapper discordClientWrapper,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.guildSettingsService = guildSettingsService;
    }
    
    public async Task ExecuteAsync()
    {
        var bedgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Bedge");
        await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, $"{bedgeEmote}");
    }
    
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IGuildSettingsService guildSettingsService;
}