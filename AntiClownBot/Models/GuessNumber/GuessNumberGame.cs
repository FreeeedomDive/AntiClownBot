using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntiClownBot.Models.User.Inventory.Items;
using DSharpPlus;

namespace AntiClownBot.Models.GuessNumber
{
    public class GuessNumberGame
    {
        public ulong GuessNumberGameMessageId;
        public Dictionary<ulong, int> Users;
        public bool IsJoinable;
        private int generatedNumber;

        public void Join(SocialRatingUser user, int number)
        {
            if (Users.ContainsKey(user.DiscordId) || !IsJoinable)
            {
                return;
            }

            Users.Add(user.DiscordId, number);
        }

        public GuessNumberGame(ulong id)
        {
            generatedNumber = Randomizer.GetRandomNumberBetween(1, 6);
            Users = new Dictionary<ulong, int>();
            GuessNumberGameMessageId = id;
            IsJoinable = true;
        }

        public async void MakeResult()
        {
            IsJoinable = false;
            await Utility.Client
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync($"Правильный ответ {generatedNumber}!");
            var count = 0;
            foreach (var pair in Users)
            {
                if (pair.Value == generatedNumber)
                {
                    var user = Configuration.GetConfiguration().Users[pair.Key];
                    user.AddCustomItem(new LootBox());
                    await Utility.Client
                        .Guilds[277096298761551872]
                        .GetChannel(838477706643374090)
                        .SendMessageAsync($"{user.DiscordUsername} получает Добыча коробка!");
                    count++;
                }
            }

            if (count == 0)
            {
                await Utility.Client
                    .Guilds[277096298761551872]
                    .GetChannel(838477706643374090)
                    .SendMessageAsync($"Никто не угадал {Utility.Emoji(":PogOff:")}!");
            }
        }
    }
}
