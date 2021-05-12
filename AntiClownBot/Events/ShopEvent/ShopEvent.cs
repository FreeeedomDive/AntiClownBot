using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AntiClownBot.Events.ShopEvent
{
    public class ShopEvent : BaseEvent
    {
        public override void ExecuteAsync()
        {
            var thread = new Thread(Run)
            {
                IsBackground = true
            };
            thread.Start();
        }
        private async void Run()
        {
            Config.Market = new Models.Shop.Shop();
            var text = BackStory();
            var message = await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(text);
            await message.CreateReactionAsync(Utility.Emoji(":dog:"));
            await message.CreateReactionAsync(Utility.Emoji(":RainbowPls:"));
            await message.CreateReactionAsync(Utility.Emoji(":rice:"));
            await message.CreateReactionAsync(Utility.Emoji(":monkaGIGA:"));
            Config.Market.ShopMessageId = message.Id;
            Config.Save();
            Thread.Sleep(2 * 60 * 60 * 1000);
            Config.Market = null;
        }
        protected override string BackStory()
        {
            return $"Магазин ОТКРЫТЬСЯ!\n" +
                "Всё по 1000! Но покупать каждый 'предмет' только 1 раз!\n" +
                "Магазин работать 2 часа" +
                $"{Utility.Emoji(":dog:")} - собака жена\n" +
                $"{Utility.Emoji(":RainowPls:")} - кошка жена\n" +
                $"{Utility.Emoji(":rice:")} - миска рис" +
                $"{Utility.Emoji(":monkaGIGA:")} - гигабайт интернет";
        }
    }
}
