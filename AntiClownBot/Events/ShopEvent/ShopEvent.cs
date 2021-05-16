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
            await message.CreateReactionAsync(Utility.Emoji(":HACKERJAMS:"));
            var secondmessage = await DiscordClient
                .Guilds[277096298761551872]
                .GetChannel(838477706643374090)
                .SendMessageAsync(BackStory2());
            await secondmessage.CreateReactionAsync(Utility.Emoji(":dog:"));
            await secondmessage.CreateReactionAsync(Utility.Emoji(":RainbowPls:"));
            await secondmessage.CreateReactionAsync(Utility.Emoji(":rice:"));
            await secondmessage.CreateReactionAsync(Utility.Emoji(":HACKERJAMS:"));
            await secondmessage.CreateReactionAsync(Utility.Emoji(":BONK:"));
            await secondmessage.CreateReactionAsync(Utility.Emoji(":cykaPls:"));
            Config.Market.ShopBuyMessageId = message.Id;
            Config.Market.ShopSellMessageId = secondmessage.Id;
            Config.Save();
            await Task.Delay(40 * 60 * 1000);
            Config.Market = null;
        }

        protected override string BackStory()
        {
            return "Магазин ОТКРЫТЬСЯ!\n" +
                   "Всё по 1000! Но покупать каждый 'предмет' только 1 раз!\n" +
                   "Магазин работать 40 минут\n" +
                   $"{Utility.Emoji(":dog:")} - собака жена\n" +
                   $"{Utility.Emoji(":RainbowPls:")} - кошка жена\n" +
                   $"{Utility.Emoji(":rice:")} - миска рис\n" +
                   $"{Utility.Emoji(":HACKERJAMS:")} - гигабайт интернет";
        }
        protected string BackStory2()
        {
            return "Продавайте свои предметы!\n" +
                   $"{Utility.Emoji(":dog:")} - собака жена +500\n" +
                   $"{Utility.Emoji(":RainbowPls:")} - кошка жена +500\n" +
                   $"{Utility.Emoji(":rice:")} - миска рис +500\n" +
                   $"{Utility.Emoji(":HACKERJAMS:")} - гигабайт интернет +500\n" +
                   $"{Utility.Emoji(":BONK:")} - нефритовый стержень -2000\n" +
                   $"{Utility.Emoji(":cykaPls:")} - {Utility.ItemToString(InventoryItem.CommunismPoster)} -2000";
        }
    }
}