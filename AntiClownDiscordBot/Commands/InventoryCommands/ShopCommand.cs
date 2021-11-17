using AntiClownBot.Helpers;
using AntiClownBot.Models.Shop;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.InventoryCommands
{
    public class ShopCommand : BaseCommand
    {
        public ShopCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var message = await e.Message.RespondAsync(Shop.CreateLoadingShopEmbed());
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
            await message.CreateReactionAsync(Utility.Emoji(":question:"));
        }

        public override string Help() => Shop.Help;
    }
}