using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.SocialRatingCommands
{
    public class TributeCommand : ICommand
    {
        public TributeCommand(
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

        public async Task Execute(MessageCreateEventArgs e)
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            if (e.Channel.Id != guildSettings.TributeChannelId && e.Channel.Id != guildSettings.HiddenTestChannelId)
            {
                var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
                var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
                var tributeChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.TributeChannelId);
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
                return;
            }

            var tributeResult = await apiClient.Users.TributeAsync(e.Author.Id);
            var tributeEmbed = await tributeService.TryMakeEmbedForTribute(tributeResult);
            if (tributeEmbed != null)
                await discordClientWrapper.Messages.RespondAsync(e.Message, tributeEmbed);
        }

        public Task<string> Help()
        {
            return Task.FromResult(
                "Преподношение императору XI для увеличения (или уменьшения) своего баланса скам койнов" +
                "\nДефолтный кулдаун 1 час, понижается наличием интернета"
            );
        }

        public string Name => "tribute";
        public bool IsObsolete => false;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly TributeService tributeService;
        private readonly IGuildSettingsService guildSettingsService;
    }
}