using AntiClownBot.Models.User.Inventory.Items;
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
            await message.CreateReactionAsync(Utility.Emoji(":PepegaCredit:"));
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
                   "Но покупать каждый 'предмет' только 1 раз!\n" +
                   "Магазин работать 40 минут\n" +
                   $"{Utility.Emoji(":dog:")} - собака жена {new DogWife().Price}\n" +
                   $"{Utility.Emoji(":RainbowPls:")} - кошка жена {new CatWife().Price}\n" +
                   $"{Utility.Emoji(":rice:")} - миска рис {new RiceBowl().Price}\n" +
                   $"{Utility.Emoji(":HACKERJAMS:")} - гигабайт интернет {new Gigabyte().Price}\n" +
                   $"{Utility.Emoji(":PepegaCredit:")} - лутбокс {new LootBox().Price}";
        }
        protected string BackStory2()
        {
            return "Продавайте свои предметы!\n" +
                   $"{Utility.Emoji(":dog:")} - собака жена {new DogWife().Price/2}\n" +
                   $"{Utility.Emoji(":RainbowPls:")} - кошка жена {new CatWife().Price/2}\n" +
                   $"{Utility.Emoji(":rice:")} - миска рис {new RiceBowl().Price/2}\n" +
                   $"{Utility.Emoji(":HACKERJAMS:")} - гигабайт интернет {new Gigabyte().Price/2}\n" +
                   $"{Utility.Emoji(":BONK:")} - нефритовый стержень {new JadeRod().Price/2}\n" +
                   $"{Utility.Emoji(":cykaPls:")} - коммунистический плакат {new CommunismPoster().Price/2}";
        }
    }
}