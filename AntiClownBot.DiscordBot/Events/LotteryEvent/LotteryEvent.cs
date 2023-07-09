using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.LotteryEvent
{
    public class LotteryEvent : IEvent
    {
        public LotteryEvent(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IGuildSettingsService guildSettingsService,
            ILotteryService lotteryService,
            IEventSettingsService eventSettingsService,
            IAppSettingsService appSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.guildSettingsService = guildSettingsService;
            this.lotteryService = lotteryService;
            this.eventSettingsService = eventSettingsService;
            this.appSettingsService = appSettingsService;
        }

        public async Task ExecuteAsync()
        {
            if (lotteryService.Lottery != null)
                return;
            var message = await TellBackStory();
            lotteryService.CreateLottery(message.Id);
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var noted = await emotesProvider.GetEmoteAsync("NOTED");
            var text = (DateTime.Now.IsNightTime() || !appSettingsService.GetSettings().PingOnEvents
                           ? ""
                           : "@everyone ") +
                       $"Начинаем лотерею! Для участия нажмите на смайлик {noted} под сообщением или через команду !lottery\n" +
                       "Здесь можно выиграть много scam-койнов!\n" +
                       "Вся необходимая информация о лотерее доступна по команде '!help lottery'\n" +
                       $"Начнём подводить итоги через {eventSettingsService.GetEventSettings().LotteryStartDelayInMinutes} минут";

            var message = await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, text);
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, noted);

            return message;
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly ILotteryService lotteryService;
        private readonly IEventSettingsService eventSettingsService;
        private readonly IAppSettingsService appSettingsService;
    }
}