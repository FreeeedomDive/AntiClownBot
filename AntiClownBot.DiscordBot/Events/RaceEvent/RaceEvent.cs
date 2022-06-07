using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.EventServices;
using AntiClownDiscordBotVersion2.Models.Race;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.RaceEvent
{
    public class RaceEvent : IEvent
    {
        public RaceEvent(
            IDiscordClientWrapper discordClientWrapper,
            IGuildSettingsService guildSettingsService,
            IRaceService raceService,
            IEventSettingsService eventSettingsService,
            IAppSettingsService appSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.guildSettingsService = guildSettingsService;
            this.raceService = raceService;
            this.eventSettingsService = eventSettingsService;
            this.appSettingsService = appSettingsService;
        }

        public async Task ExecuteAsync()
        {
            if (raceService.Race != null) return;

            var joinableMessage = await TellBackStory();
            await raceService.CreateRaceAsync(joinableMessage.Id);

            var delayInMinutes = eventSettingsService.GetEventSettings().RaceStartDelayInMinutes;
            await Task.Delay(delayInMinutes * 60 * 1000);
            raceService.Race.StartRace();
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var delayInMinutes = eventSettingsService.GetEventSettings().RaceStartDelayInMinutes;
            var messageContent = (DateTime.Now.IsNightTime()  || !appSettingsService.GetSettings().PingOnEvents ? "" : "@everyone ") + "Начинаем гоночку!!!" +
                                 "\nСоревнуйтесь друг с другом и, главное, со мной, ведь тот, кто сможет обойти меня (и попасть в топ-10), получит социальный рейтинг" +
                                 $"\nРаспределение рейтинга - {string.Join(", ", RaceModel.Points)}" +
                                 "\nДля получения рейтинга обязательно нужно быть впереди меня" +
                                 $"\nЖми {await discordClientWrapper.Emotes.FindEmoteAsync("monkaSTEER")}, чтобы участвовать." +
                                 $"\nСтарт через {delayInMinutes} минут\n";
            var message = await discordClientWrapper.Messages.SendAsync(
                guildSettingsService.GetGuildSettings().BotChannelId,
                messageContent
            );
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("monkaSTEER"));

            return message;
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;

        public List<IEvent> RelatedEvents => new();

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IRaceService raceService;
        private readonly IEventSettingsService eventSettingsService;
        private readonly IAppSettingsService appSettingsService;
    }
}