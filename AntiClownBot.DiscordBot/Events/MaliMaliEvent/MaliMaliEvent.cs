using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.MaliMaliEvent
{
    public class MaliMaliEvent : IEvent
    {
        public MaliMaliEvent(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IGuildSettingsService guildSettingsService,
            IRandomizer randomizer,
            ILogger logger
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.guildSettingsService = guildSettingsService;
            this.randomizer = randomizer;
            this.logger = logger;
        }

        public async Task ExecuteAsync()
        {
            var channels = await discordClientWrapper.Guilds.GetGuildChannels();
            var joinableChannels = channels.Where(ch => ch.Type == ChannelType.Voice && ch.Users.ToList().Count > 1).ToArray();
            if (joinableChannels.Length == 0)
            {
                await discordClientWrapper.Messages.SendAsync(
                    guildSettingsService.GetGuildSettings().BotChannelId,
                    $"Никто не пришел на MALI-MALI-фанвстречу {await discordClientWrapper.Emotes.FindEmoteAsync("BibleThump")}"
                );
                return;
            }

            await TellBackStory();

            var channel = joinableChannels.SelectRandomItem(randomizer);
            var isConnected = await discordClientWrapper.Voice.TryConnectAsync(channel);
            if (!isConnected)
            {
                await discordClientWrapper.Voice.DisconnectAsync();
                await discordClientWrapper.Voice.TryConnectAsync(channel);
            }

            try
            {
                discordClientWrapper.Voice.ClearQueue();
                while (discordClientWrapper.Voice.IsPlaying())
                {
                    await discordClientWrapper.Voice.WaitForPlaybackFinishAsync();
                }
                await discordClientWrapper.Voice.PlaySoundAsync("zapret.mp3");

                var botId = await discordClientWrapper.Members.GetBotIdAsync();
                var voiceUsers = await discordClientWrapper.Voice.GetVoiceStatesInChannelAsync(channel.Id);
                var voiceUsersExceptBots = voiceUsers.Where(u => u.User.Id != botId).ToArray();
                foreach (var voiceState in voiceUsersExceptBots)
                {
                    if (voiceState.Channel != channel) continue;
                    var member = voiceState.Member;
                    await discordClientWrapper.Members.ModifyAsync(member, model => model.Muted = true);
                }

                new Thread(async () =>
                {
                    await Task.Delay(60 * 1000);
                    foreach (var voiceState in voiceUsersExceptBots)
                    {
                        if (voiceState.Channel != channel) continue;

                        var member = voiceState.Member;
                        await discordClientWrapper.Members.ModifyAsync(member, model => model.Muted = false);

                        await apiClient.Users.ChangeUserRatingAsync(member.Id, randomizer.GetRandomNumberBetween(100, 250), "MALI-MALI");
                    }
                }).Start();

                await discordClientWrapper.Voice.PlaySoundAsync("malimali.mp3");
            }
            catch (Exception ex)
            {
                logger.Error("Мали-мали обосрался", ex);
            }
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var pomLeftEmote = await discordClientWrapper.Emotes.FindEmoteAsync("pomLeft");
            var floppaEmote = await discordClientWrapper.Emotes.FindEmoteAsync("FLOPPA");
            var pomRight = await discordClientWrapper.Emotes.FindEmoteAsync("pomRight");
            var messageContent =
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}\n" +
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}\n" +
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}\n" +
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}\n" +
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}\n" +
                $"{pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight} MALI MALI {pomLeftEmote} {pomLeftEmote} {floppaEmote} {pomRight} {pomRight}";

            var message = await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContent);

            return message;
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IRandomizer randomizer;
        private readonly ILogger logger;
    }
}