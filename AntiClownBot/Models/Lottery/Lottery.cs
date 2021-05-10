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
        public enum LotteryEmotes
        {
            //Плохие эмоты
            weirdChamp,
            sadKEK,
            Sadge,
            PogOff,
            pepeLaugh,
            KEKW,
            bonk,
            BibleThump,
            OMEGAKEK,
            PepegaGun,
            peepoClown,
            //Хорошие эмоты
            PogYou,
            poggers,
            pauseChamp,
            olyashGasm,
            OkayChamp,
            monkaX,
            funnyChamp,
            BASED,
            AYAYA,
            YEPPING,
            RainbowPls,
            peepoClap,
            PATREGO
        }
        public Lottery()
        {
            _configuration = Configuration.GetConfiguration();
            DiscordClient = Utility.Client;
            participants = new Queue<ulong>();
            IsJoinable = true;
            var thread = new Thread(StartEvent)
            {
                IsBackground = true
            };
            thread.Start();
        }
        public Queue<ulong> participants;
        public string Join(SocialRatingUser user)
        {
            if (user.SocialRating < 250)
                return $"Недостаточно кредитов у {user.DiscordUsername}";
            user.DecreaseRating(250);
            participants.Enqueue(user.DiscordId);
            return $"{user.DiscordUsername} приобрёл билет";
        }
        private async void StartEvent()
        {
            Thread.Sleep((int)20 * 60 * 1000);
            IsJoinable = false;
            foreach(var user in Start())
            {
                var strBuilder = new StringBuilder();
                strBuilder.Append($"{user.User.DiscordUsername}:\n");
                var message = await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(strBuilder.ToString());
                foreach(var emote in user.Emotes)
                {
                    strBuilder.Append($"{Utility.StringEmoji($":{emote}:")} ");
                    await message.ModifyAsync(strBuilder.ToString());
                    Thread.Sleep(1000);
                }
                strBuilder.Append($"\nТы получил {user.Value}");
                if(user.Value < 0)
                {
                    user.Value *= -1;
                    user.User.DecreaseRating(user.Value);
                }
                else
                {
                    user.User.IncreaseRating(user.Value);
                }    
            }
        }
        public IEnumerable<LotteryUser> Start()
        {
            var allEmotes = Enum.GetValues(typeof(LotteryEmotes));
            while (participants.Count != 0)
            {
                var user = _configuration.Users[participants.Dequeue()];
                var emotes = new LotteryEmotes[5];
                var tempDictionary = new Dictionary<LotteryEmotes, int>();
                for(var i = 0; i < 5; i++)
                {
                    var emote = (LotteryEmotes) allEmotes.GetValue(Randomizer.GetRandomNumberBetween(0, allEmotes.Length - 1));
                    emotes[i] = emote;
                    if(tempDictionary.ContainsKey(emote))
                    {
                        tempDictionary[emote]++;
                    }
                    else
                    {
                        tempDictionary.Add(emote, 1);
                    }
                }
                var value = 0;
                foreach(var emote in tempDictionary)
                {
                    value += GetEmotesValue(emote.Key, emote.Value);
                }
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
        private int EmoteToInt(LotteryEmotes emote)
        {
            return emote switch
            {
                LotteryEmotes.weirdChamp => -10,
                LotteryEmotes.sadKEK => -20,
                LotteryEmotes.Sadge => -10,
                LotteryEmotes.PogOff => -50,
                LotteryEmotes.pepeLaugh => -30,
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
                LotteryEmotes.monkaX => 40,
                LotteryEmotes.funnyChamp => 10,
                LotteryEmotes.BASED => 40,
                LotteryEmotes.AYAYA => 40,
                LotteryEmotes.YEPPING => 10,
                LotteryEmotes.RainbowPls => 30,
                LotteryEmotes.peepoClap => 20,
                LotteryEmotes.PATREGO => 75,
                _ => throw new ArgumentOutOfRangeException(nameof(emote), emote, null)
            };
        }
        private int GetEmotesValue(LotteryEmotes emote, int count)
        {
            switch(count)
            {
                case 1:
                    return EmoteToInt(emote);
                case 2:
                    return EmoteToInt(emote) * 5;
                case 3:
                    return EmoteToInt(emote) * 10;
                case 4:
                    return EmoteToInt(emote) * 50;
                case 5:
                    return EmoteToInt(emote) * 100;
                default:
                    return 0;
            }
        }
    }
}
