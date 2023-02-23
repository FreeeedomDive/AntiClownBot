using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.SocialRating;

public class TributeCommandModule : ApplicationCommandModule
{
    public TributeCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        TributeService tributeService,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.tributeService = tributeService;
        this.guildSettingsService = guildSettingsService;
    }

    [SlashCommand("tribute", "Подношение императору XI")]
    public async Task Tribute(InteractionContext context)
    {
        var guildSettings = guildSettingsService.GetGuildSettings();
        if (context.Channel.Id != guildSettings.TributeChannelId && context.Channel.Id != guildSettings.HiddenTestChannelId)
        {
            var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
            var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
            var tributeChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.TributeChannelId);
            await discordClientWrapper.Messages.RespondAsync(context, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
            return;
        }

        var tributeResult = await apiClient.Users.TributeAsync(context.User.Id);
        var tributeEmbed = await tributeService.TryMakeEmbedForTribute(tributeResult);
        if (tributeEmbed != null)
        {
            await discordClientWrapper.Messages.RespondAsync(context, tributeEmbed);
        }
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly TributeService tributeService;
    private readonly IGuildSettingsService guildSettingsService;
}