using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.InventoryCommands
{
    public class ShopCommand : ICommand
    {
        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IShopService shopService;
        private readonly IRandomizer randomizer;

        public ShopCommand(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IShopService shopService,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.shopService = shopService;
            this.randomizer = randomizer;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var member = await discordClientWrapper.Members.GetAsync(e.Author.Id);
            var shop = new Shop(discordClientWrapper, apiClient, randomizer)
            {
                UserId = e.Author.Id,
                Member = member
            };
            var message = await discordClientWrapper.Messages.RespondAsync(e.Message, await shop.CreateLoadingShopEmbed());
            shop.BindToMessage(message);
            shopService.Create(message.Id, shop);
            await shop.UpdateShopMessage();

            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("one"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("two"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("three"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("four"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("five"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("pepeSearching"));
        }

        public async Task<string> Help()
        {
            var pepeSearching = await discordClientWrapper.Emotes.FindEmoteAsync("pepeSearching");
            var coggers = await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS");
            return "Создает новое сообщение с персональным магазином для пользователя" +
                   "\nМагазин доступен всегда, можно взаимодействовать с персональным сообщением магазина" +
                   "\nИзначально предметы в магазине скрыты, так что есть риск вслепую покупать предмет" +
                   "\nПо умолчанию дается 2 бесплатных распознавателя предметов на день, затем распознавание предмета будет стоить 40% от его стоимости" +
                   $"\nНажатый {pepeSearching} = распознаватель, ненажатый {pepeSearching} = покупка" +
                   "\nПокупка предметов - через кнопки, соответствующие слотам магазина" +
                   $"\nДля реролла магазина нажмите {coggers}" +
                   "\nПри покупке предмета он неактивен, его нужног активировать в интерфейсе команды !inventory" +
                   "\nПо цвету блока сообщения можно определить максимальную редкость одного из предметов в магазине";
        }

        public string Name => "shop";
    }
}