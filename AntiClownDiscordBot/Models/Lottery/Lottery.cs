using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiClownBot.Models.Lottery
{
    public class Lottery
    {
        private Configuration _configuration;
        protected static DiscordClient DiscordClient;
        public bool IsJoinable;
        public ulong LotteryMessageId;

        public enum LotteryEmote
        {
            //Плохие эмоты
            weirdChamp,
            sadKEK,
            Sadge,
            PogOff,
            pigRoll,
            KEKW,
            bonk,
            BibleThump,
            OMEGAKEK,
            PepegaGun,
            peepoClown,
            Kekega,

            //Хорошие эмоты
            PogYou,
            poggers,
            pauseChamp,
            olyashGasm,
            OkayChamp,
            BOOBA,
            funnyChamp,
            BASED,
            AYAYA,
            YEPPING,
            RainbowPls,
            peepoClap,
            PATREGO,
            popCat
        }

        public static List<LotteryEmote> GetAllEmotes()
        {
            return Enum.GetValues(typeof(LotteryEmote)).Cast<LotteryEmote>().ToList();
        }

        public Lottery()
        {
            DiscordClient = Utility.Client;
            Participants = new Queue<ulong>();
            IsJoinable = true;
            var thread = new Thread(StartEvent)
            {
                IsBackground = true
            };
            thread.Start();
        }

        public Queue<ulong> Participants;

        public string Join(ulong userId)
        {
            _configuration ??= Configuration.GetConfiguration();
            Participants.Enqueue(userId);
            _configuration.Save();
            var member = DiscordClient.Guilds[Constants.GuildId].GetMemberAsync(userId).Result;
            return $"{member.Nickname} теперь участвует в лотерее";
        }

        private async void StartEvent()
        {
            var allEmotes = GetAllEmotes();
            await Task.Delay(15 * 60 * 1000);
            _configuration ??= Configuration.GetConfiguration();
            DiscordClient ??= Utility.Client;
            IsJoinable = false;
            foreach (var user in Start())
            {
                var member = Configuration.GetServerMember(user.UserId);
                var lastMessage = $"{member.Nickname}:\n";

                var message = await DiscordClient
                    .Guilds[277096298761551872]
                    .GetChannel(838477706643374090)
                    .SendMessageAsync(lastMessage);

                var emotesToRoll = 7;
                while (emotesToRoll > 0)
                {
                    var currentEmote = 7 - emotesToRoll;
                    var strBuilder = new StringBuilder();
                    strBuilder.Append($"{member.ServerOrUsername()}:\n");

                    for (var rolledEmotes = 0; rolledEmotes < currentEmote; rolledEmotes++)
                    {
                        strBuilder.Append($"{Utility.StringEmoji($":{user.Emotes[rolledEmotes]}:")} ");
                    }
                    
                    LotteryEmote rollingEmote;
                    if (Randomizer.GetRandomNumberBetween(0, 4) == 0)
                    {
                        rollingEmote = allEmotes.SelectRandomItem();
                    }
                    else
                    {
                        rollingEmote = user.Emotes[currentEmote];
                        emotesToRoll--;
                    }
                    strBuilder.Append($"   {Utility.StringEmoji($":{rollingEmote}:")} ");
                    
                    for (var remainingEmote = currentEmote + 1; remainingEmote < 7; remainingEmote++)
                    {
                        strBuilder.Append($"{Utility.StringEmoji($":{allEmotes.SelectRandomItem()}:")} ");
                    }

                    lastMessage = strBuilder.ToString();
                    await message.ModifyAsync(lastMessage);
                    Thread.Sleep(1000);
                }

                var builder = new StringBuilder();
                builder.Append($"{member.ServerOrUsername()}:\n");
                builder.Append(string.Join(" ", user.Emotes.Select(emote => $"{Utility.StringEmoji($":{emote}:")}")));

                builder.Append($"\nТы получил {user.Value} social credit!");
                _configuration.ChangeBalance(user.UserId, user.Value, "Лотерея");

                await message.ModifyAsync(builder.ToString());
                _configuration.Save();
            }

            _configuration.CurrentLottery = null;
            _configuration.Save();
        }

        public IEnumerable<LotteryUser> Start()
        {
            _configuration ??= Configuration.GetConfiguration();
            var allEmotes = Enum.GetValues(typeof(LotteryEmote)).Cast<LotteryEmote>().ToList();
            while (Participants.Count != 0)
            {
                var user = Participants.Dequeue();
                var emotes = new LotteryEmote[7];
                var tempDictionary = new Dictionary<LotteryEmote, int>();
                for (var i = 0; i < 7; i++)
                {
                    var emote = allEmotes.SelectRandomItem();
                    emotes[i] = emote;
                    if (tempDictionary.ContainsKey(emote))
                    {
                        tempDictionary[emote]++;
                    }
                    else
                    {
                        tempDictionary.Add(emote, 1);
                    }
                }

                var value = tempDictionary.Sum(emote => GetEmotesValue(emote.Key, emote.Value));

                yield return new LotteryUser
                {
                    UserId = user,
                    Emotes = emotes,
                    Value = value
                };
            }
        }

        public class LotteryUser
        {
            public ulong UserId;
            public LotteryEmote[] Emotes;
            public int Value;
        }

        public static int EmoteToInt(LotteryEmote emote)
        {
            return emote switch
            {
                LotteryEmote.weirdChamp => -10,
                LotteryEmote.sadKEK => -20,
                LotteryEmote.Sadge => -10,
                LotteryEmote.PogOff => -50,
                LotteryEmote.pigRoll => -30,
                LotteryEmote.KEKW => -40,
                LotteryEmote.bonk => -30,
                LotteryEmote.BibleThump => -20,
                LotteryEmote.OMEGAKEK => -40,
                LotteryEmote.PepegaGun => -75,
                LotteryEmote.peepoClown => -20,
                LotteryEmote.PogYou => 40,
                LotteryEmote.poggers => 30,
                LotteryEmote.pauseChamp => 50,
                LotteryEmote.olyashGasm => 30,
                LotteryEmote.OkayChamp => 20,
                LotteryEmote.BOOBA => 40,
                LotteryEmote.funnyChamp => 10,
                LotteryEmote.BASED => 40,
                LotteryEmote.AYAYA => 40,
                LotteryEmote.YEPPING => 10,
                LotteryEmote.RainbowPls => 30,
                LotteryEmote.peepoClap => 20,
                LotteryEmote.PATREGO => 75,
                LotteryEmote.Kekega => -35,
                LotteryEmote.popCat => 50,
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
    }
}