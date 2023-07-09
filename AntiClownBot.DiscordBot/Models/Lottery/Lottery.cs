using System.Diagnostics.CodeAnalysis;
using System.Text;
using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Settings.EventSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.UserBalance;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;

namespace AntiClownDiscordBotVersion2.Models.Lottery
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    public class Lottery
    {
        public Lottery(
            IDiscordClientWrapper discordClientWrapper,
            IEmotesProvider emotesProvider,
            IRandomizer randomizer,
            IEventSettingsService eventSettingsService,
            IGuildSettingsService guildSettingsService,
            IApiClient apiClient,
            IUserBalanceService userBalanceService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.emotesProvider = emotesProvider;
            this.randomizer = randomizer;
            this.eventSettingsService = eventSettingsService;
            this.guildSettingsService = guildSettingsService;
            this.apiClient = apiClient;
            this.userBalanceService = userBalanceService;
        }

        public Lottery Create(ulong messageId)
        {
            MessageId = messageId;
            Participants = new Queue<ulong>();
            IsJoinable = true;
            var thread = new Thread(StartEvent)
            {
                IsBackground = true
            };
            thread.Start();

            return this;
        }

        public enum LotteryEmote
        {
            Starege,
            KEKWiggle,
            FLOPPA,
            Applecatrun,
            cykaPls,
            PaPaTuTuWaWa,
            PolarStrut,
            HACKERMANS,
            PATREGO,
            triangD
        }

        public static List<LotteryEmote> GetAllEmotes()
        {
            return Enum.GetValues(typeof(LotteryEmote)).Cast<LotteryEmote>().ToList();
        }

        public async Task<string> Join(ulong userId)
        {
            Participants.Enqueue(userId);
            var member = await discordClientWrapper.Members.GetAsync(userId);
            return $"{member.ServerOrUserName()} теперь участвует в лотерее";
        }

        private async void StartEvent()
        {
            var allEmotes = GetAllEmotes();
            var discordEmotes = await
                Task.WhenAll(allEmotes.Select(emote => emotesProvider.GetEmoteAsync(emote.ToString())));
            await Task.Delay(eventSettingsService.GetEventSettings().LotteryStartDelayInMinutes * 60 * 1000);
            IsJoinable = false;
            foreach (var user in GenerateLotteryResults())
            {
                var member = await discordClientWrapper.Members.GetAsync(user.UserId);
                var lastMessage = $"{member.ServerOrUserName()}:\n";

                var message = await discordClientWrapper.Messages.SendAsync(guildSettingsService.GetGuildSettings().BotChannelId, lastMessage);

                var emotesToRoll = 7;
                while (emotesToRoll > 0)
                {
                    var currentEmote = 7 - emotesToRoll;
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"{member.ServerOrUserName()}:\n");

                    for (var rolledEmotes = 0; rolledEmotes < currentEmote; rolledEmotes++)
                    {
                        var emoteName = user.Emotes[rolledEmotes].ToString();
                        var discordEmote = discordEmotes.First(e => e.Name.Equals(emoteName));
                        strBuilder.Append($"{discordEmote} ");
                    }

                    LotteryEmote rollingEmote;
                    if (randomizer.FlipACoin())
                    {
                        rollingEmote = user.Emotes[currentEmote];
                        emotesToRoll--;
                    }
                    else
                    {
                        rollingEmote = allEmotes.SelectRandomItem(randomizer);
                    }

                    var rollingDiscordEmote = discordEmotes.First(e => e.Name.Equals(rollingEmote.ToString()));
                    strBuilder.Append($"   {rollingDiscordEmote} ");

                    for (var remainingEmote = currentEmote + 1; remainingEmote < 7; remainingEmote++)
                    {
                        var randomEmote = allEmotes.SelectRandomItem(randomizer).ToString();
                        var randomDiscordEmote = discordEmotes.First(e => e.Name.Equals(randomEmote));
                        strBuilder.Append($"{randomDiscordEmote} ");
                    }

                    lastMessage = strBuilder.ToString();
                    await discordClientWrapper.Messages.ModifyAsync(message, lastMessage);
                    Thread.Sleep(1000);
                }

                var builder = new StringBuilder();
                builder.Append($"{member.ServerOrUserName()}:\n");
                builder.Append(string.Join(" ", user.Emotes.Select(emote => $"{discordEmotes.First(e => e.Name.Equals(emote.ToString()))}")));

                builder.Append($"\nТы получил {user.Value} scam coins!");

                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(user.UserId, user.Value, "Лотерея");

                await discordClientWrapper.Messages.ModifyAsync(message, builder.ToString());
            }

            OnLotteryEnd();
        }

        private IEnumerable<LotteryUser> GenerateLotteryResults()
        {
            var allEmotes = Enum.GetValues(typeof(LotteryEmote)).Cast<LotteryEmote>().ToList();
            while (Participants.Count != 0)
            {
                var user = Participants.Dequeue();
                var emotes = new LotteryEmote[7];
                var emotesCounter = new Dictionary<LotteryEmote, int>();
                for (var i = 0; i < 7; i++)
                {
                    var emote = allEmotes.SelectRandomItem(randomizer);
                    emotes[i] = emote;
                    if (emotesCounter.ContainsKey(emote))
                    {
                        emotesCounter[emote]++;
                    }
                    else
                    {
                        emotesCounter.Add(emote, 1);
                    }
                }

                var value = emotesCounter.Sum(emote => GetEmotesValue(emote.Key, emote.Value));

                yield return new LotteryUser
                {
                    UserId = user,
                    Emotes = emotes,
                    Value = value
                };
            }
        }

        private class LotteryUser
        {
            public ulong UserId;
            public LotteryEmote[] Emotes;
            public int Value;
        }

        public static int EmoteToInt(LotteryEmote emote)
        {
            return emote switch
            {
                LotteryEmote.Starege => 0,
                LotteryEmote.KEKWiggle => 5,
                LotteryEmote.FLOPPA => 10,
                LotteryEmote.Applecatrun => 15,
                LotteryEmote.cykaPls => 20,
                LotteryEmote.PaPaTuTuWaWa => 25,
                LotteryEmote.PolarStrut => 30,
                LotteryEmote.HACKERMANS => 35,
                LotteryEmote.PATREGO => 40,
                LotteryEmote.triangD => 50,
                _ => throw new ArgumentOutOfRangeException(nameof(emote), emote, null)
            };
        }

        private static int GetEmotesValue(LotteryEmote emote, int count)
        {
            return count switch
            {
                1 => EmoteToInt(emote),
                2 => EmoteToInt(emote) * 5,
                3 => EmoteToInt(emote) * 10,
                4 => EmoteToInt(emote) * 50,
                >= 5 => EmoteToInt(emote) * 100,
                _ => 0
            };
        }

        public Queue<ulong> Participants;
        public bool IsJoinable { get; private set; }
        public ulong MessageId { get; private set; }
        public Action OnLotteryEnd { get; init; }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IEmotesProvider emotesProvider;
        private readonly IRandomizer randomizer;
        private readonly IEventSettingsService eventSettingsService;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IApiClient apiClient;
        private readonly IUserBalanceService userBalanceService;
    }
}