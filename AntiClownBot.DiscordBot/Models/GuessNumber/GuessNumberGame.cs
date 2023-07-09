using System.Text;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Models.GuessNumber
{
    public class GuessNumberGame
    {
        public GuessNumberGame(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IGuildSettingsService guildSettingsService,
            IRandomizer randomizer,
            IApiClient apiClient
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.guildSettingsService = guildSettingsService;
            this.randomizer = randomizer;
            this.apiClient = apiClient;
        }

        public GuessNumberGame Create()
        {
            generatedNumber = randomizer.GetRandomNumberBetween(1, 6);
            Users = new Dictionary<ulong, int>();
            IsJoinable = true;

            return this;
        }

        public void Join(ulong userId, int number)
        {
            if (Users.ContainsKey(userId) || !IsJoinable)
            {
                return;
            }

            Users.Add(userId, number);
        }

        public async Task MakeResult()
        {
            IsJoinable = false;
            var count = 0;
            var sb = new StringBuilder($"Правильный ответ {generatedNumber}!\n");
            foreach (var (userId, userGuess) in Users)
            {
                if (userGuess != generatedNumber)
                {
                    continue;
                }

                await apiClient.Items.AddLootBoxAsync(userId);
                var member = await discordClientWrapper.Members.GetAsync(userId);
                sb.Append($"\n{member.ServerOrUserName()} получает добычу-коробку!");
                count++;
            }

            if (count == 0)
            {
                sb.Append($"\nНикто не угадал {await emotesProvider.GetEmoteAsTextAsync("peepoFinger")}!");
            }

            await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, sb.ToString());
            OnGameEnd();
        }

        public ulong GuessNumberGameMessageMessageId { get; init; }
        public Dictionary<ulong, int> Users { get; set; }
        public bool IsJoinable { get; set; }
        public Action OnGameEnd { get; init; }

        private int generatedNumber;
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IRandomizer randomizer;
        private readonly IApiClient apiClient;
    }
}