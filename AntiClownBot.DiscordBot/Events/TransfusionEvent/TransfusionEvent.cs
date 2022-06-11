using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Events.TransfusionEvent
{
    public class TransfusionEvent : IEvent
    {
        public TransfusionEvent(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IRandomizer randomizer,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.randomizer = randomizer;
            this.guildSettingsService = guildSettingsService;
        }

        public async Task ExecuteAsync()
        {
            await TellBackStory();
        }

        public async Task<DiscordMessage> TellBackStory()
        {
            var theRichestUser = await apiClient.Users.GetRichestUserAsync();

            var exchange = randomizer.GetRandomNumberBetween(50, 100);
            var exchangeUser = theRichestUser;
            while (exchangeUser == theRichestUser)
            {
                var users = await apiClient.Users.GetAllUsersAsync();
                exchangeUser = users.Users.SelectRandomItem(randomizer);
            }

            var richestMember = await discordClientWrapper.Members.GetAsync(theRichestUser);
            var exchangeMember = await discordClientWrapper.Members.GetAsync(exchangeUser);

            var messageContent = "Я решил выделить немного кредитов рандомному челу, " +
                                 "но свой бюджет я тратить не буду, возьму из кармана самого богатого " +
                                 $"{await discordClientWrapper.Emotes.FindEmoteAsync("MEGALUL")} " +
                                 $"{await discordClientWrapper.Emotes.FindEmoteAsync("point_right")} " +
                                 $"{richestMember.ServerOrUserName()}. " +
                                 $"Отдай {exchangeMember.ServerOrUserName()} {exchange} social credits"; 

            await apiClient.Users.ChangeUserRatingAsync(theRichestUser, -exchange, "Эвент перекачки");
            await apiClient.Users.ChangeUserRatingAsync(exchangeUser, exchange, "Эвент перекачки");
            
            return await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, messageContent);
        }

        public bool HasRelatedEvents() => RelatedEvents.Count > 0;
        public List<IEvent> RelatedEvents => new();
        
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IRandomizer randomizer;
        private readonly IGuildSettingsService guildSettingsService;
    }
}