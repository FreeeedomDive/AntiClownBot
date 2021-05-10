using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public enum LotteryEmotes
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

        public static List<LotteryEmotes> GetAllEmotes()
        {
            return Enum.GetValues(typeof(LotteryEmotes)).Cast<LotteryEmotes>().ToList();
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

        public string Join(SocialRatingUser user)
        {
            _configuration ??= Configuration.GetConfiguration();
            Participants.Enqueue(user.DiscordId);
            _configuration.Save();
            return $"{user.DiscordUsername} теперь участвует в лотерее";
        }

        private async void StartEvent()
        {
            await Task.Delay(20 * 60 * 1000);
            _configuration ??= Configuration.GetConfiguration();
            DiscordClient ??= Utility.Client;
            IsJoinable = false;
            foreach (var user in Start())
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append($"{user.User.DiscordUsername}:\n");
                var message = await DiscordClient
                    .Guilds[277096298761551872]
                    .GetChannel(838477706643374090)
                    .SendMessageAsync(strBuilder.ToString());
                foreach (var emote in user.Emotes)
                {
                    strBuilder.Append($"{Utility.StringEmoji($":{emote}:")} ");
                    await message.ModifyAsync(strBuilder.ToString());
                    Thread.Sleep(1000);
                }

                strBuilder.Append($"\nТы получил {user.Value} social credit!");
                if (user.Value < 0)
                {
                    user.Value *= -1;
                    var items = user.User.DecreaseRating(user.Value);
                    foreach (var item in items)
                    {
                        strBuilder.Append($"\n{user.User.DiscordUsername} теряет {Utility.ItemToString(item)}!");
                    }
                }
                else
                {
                    var items = user.User.IncreaseRating(user.Value);
                    foreach (var item in items)
                    {
                        strBuilder.Append($"\n{user.User.DiscordUsername} получает {Utility.ItemToString(item)}!");
                    }
                }

                await message.ModifyAsync(strBuilder.ToString());
                _configuration.Save();
            }

            _configuration.CurrentLottery = null;
            _configuration.Save();
        }

        public IEnumerable<LotteryUser> Start()
        {
            _configuration ??= Configuration.GetConfiguration();
            var allEmotes = Enum.GetValues(typeof(LotteryEmotes)).Cast<LotteryEmotes>().ToList();
            while (Participants.Count != 0)
            {
                var user = _configuration.Users[Participants.Dequeue()];
                var emotes = new LotteryEmotes[7];
                var tempDictionary = new Dictionary<LotteryEmotes, int>();
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
                    User = user,
                    Emotes = emotes,
                    Value = value
                };
            }
        }

        public class LotteryUser
        {
            public SocialRatingUser User;
            public LotteryEmotes[] Emotes;
            public int Value;
        }

        public static int EmoteToInt(LotteryEmotes emote)
        {
            return emote switch
            {
                LotteryEmotes.weirdChamp => -10,
                LotteryEmotes.sadKEK => -20,
                LotteryEmotes.Sadge => -10,
                LotteryEmotes.PogOff => -50,
                LotteryEmotes.pigRoll => -30,
                LotteryEmotes.KEKW => -40,
                LotteryEmotes.bonk => -30,
                LotteryEmotes.BibleThump => -20,
                LotteryEmotes.OMEGAKEK => -40,
                LotteryEmotes.PepegaGun => -75,
                LotteryEmotes.peepoClown => -20,
                LotteryEmotes.PogYou => 40,
                LotteryEmotes.poggers => 30,
                LotteryEmotes.pauseChamp => 50,
                LotteryEmotes.olyashGasm => 30,
                LotteryEmotes.OkayChamp => 20,
                LotteryEmotes.BOOBA => 40,
                LotteryEmotes.funnyChamp => 10,
                LotteryEmotes.BASED => 40,
                LotteryEmotes.AYAYA => 40,
                LotteryEmotes.YEPPING => 10,
                LotteryEmotes.RainbowPls => 30,
                LotteryEmotes.peepoClap => 20,
                LotteryEmotes.PATREGO => 75,
                LotteryEmotes.Kekega => -35,
                LotteryEmotes.popCat => 50,
                _ => throw new ArgumentOutOfRangeException(nameof(emote), emote, null)
            };
        }

        private static int GetEmotesValue(LotteryEmotes emote, int count)
        {
            return count switch
            {
                1 => EmoteToInt(emote),
                2 => EmoteToInt(emote) * 5,
                3 => EmoteToInt(emote) * 10,
                4 => EmoteToInt(emote) * 50,
                5 => EmoteToInt(emote) * 100,
                6 => EmoteToInt(emote) * 100,
                7 => EmoteToInt(emote) * 100,
                _ => 0
            };
        }
    }
}