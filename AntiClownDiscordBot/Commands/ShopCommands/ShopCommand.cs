using AntiClownBot.Helpers;
using AntiClownBot.Models.Shop;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.ShopCommands
{
    public class ShopCommand: BaseCommand
    {
        public ShopCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var message = await e.Message.RespondAsync(Shop.CreateLoadingEmbed());
            var shop = new Shop()
            {
                UserId = e.Author.Id,
                Message = message,
                Member = Configuration.GetServerMember(e.Author.Id)
            };
            Config.Shops[e.Author.Id] = shop;
            await shop.UpdateShopMessage();
            await message.CreateReactionAsync(Utility.Emoji(":one:"));
            await message.CreateReactionAsync(Utility.Emoji(":two:"));
            await message.CreateReactionAsync(Utility.Emoji(":three:"));
            await message.CreateReactionAsync(Utility.Emoji(":four:"));
            await message.CreateReactionAsync(Utility.Emoji(":five:"));
            await message.CreateReactionAsync(Utility.Emoji(":COGGERS:"));
            await message.CreateReactionAsync(Utility.Emoji(":pepeSearching:"));
        }

        public override string Help()
        {
            return "Создает новое сообщение с персональным магазином для пользователя" +
                   "\nМагазин доступен всегда, можно взаимодействовать с персональным сообщением магазина" +
                   "\nИзначально предметы в магазине скрыты, так что есть риск вслепую покупать предмет" +
                   "\nПо умолчанию дается 1 бесплатный распознаватель предмета на день, затем распознавание предмета будет стоить 40% от его стоимости" +
                   $"\nНажатый {Utility.Emoji(":pepeSearching:")} = распознаватель, ненажатый {Utility.Emoji(":pepeSearching:")} = покупка" +
                   "\nПокупка предметов - через кнопки, соответствующие слотам магазина" +
                   $"\nДля реролла магазина нажмите {Utility.Emoji(":COGGERS:")}" +
                   "\nПри переполнении предметов одного типа из инвентаря автоматически удаляется самый плохой по редкости предмет (или случайный из нескольких одинаковой редкости)" +
                   "\nПо цвету блока сообщения можно определить максимальную редкость одного из предметов в магазине" +
                   "\nУдачного выбивания новых предметов!";
        }
    }
}