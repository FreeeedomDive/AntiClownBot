using AntiClownBot.Helpers;
using AntiClownBot.Models.Inventory;
using ApiWrapper.Wrappers;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.InventoryCommands
{
    public class InventoryCommand : BaseCommand
    {
        public InventoryCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var message = await e.Message.RespondAsync(UserInventory.CreateLoadingInventoryEmbed());
            var inventory = new UserInventory(e.Author.Id)
            {
                Message = message,
                Member = Configuration.GetServerMember(e.Author.Id)
            };
            var embed = inventory.UpdateEmbedForCurrentPage();
            await message.ModifyAsync(embed);
            Config.Inventories[e.Author.Id] = inventory;
            await message.CreateReactionAsync(Utility.Emoji(":arrow_left:"));
            await message.CreateReactionAsync(Utility.Emoji(":arrow_right:"));
            await message.CreateReactionAsync(Utility.Emoji(":one:"));
            await message.CreateReactionAsync(Utility.Emoji(":two:"));
            await message.CreateReactionAsync(Utility.Emoji(":three:"));
            await message.CreateReactionAsync(Utility.Emoji(":four:"));
            await message.CreateReactionAsync(Utility.Emoji(":five:"));
            await message.CreateReactionAsync(Utility.Emoji(":repeat:"));
            await message.CreateReactionAsync(Utility.Emoji(":x:"));
        }

        public override string Help()
        {
            return "Получение списка всех предметов пользователя\nУправление:" +
                   "Стрелочки - переключение страниц, все предметы разбиты постранично по типам" +
                   "Выбор действия - сделать предмет активным или продать";
        }
    }
}