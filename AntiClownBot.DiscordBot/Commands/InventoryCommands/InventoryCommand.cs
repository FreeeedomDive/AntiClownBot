using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.InventoryCommands
{
    public class InventoryCommand : ICommand
    {
        public InventoryCommand(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IUserInventoryService userInventoryService,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.userInventoryService = userInventoryService;
            this.randomizer = randomizer;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var member = await discordClientWrapper.Members.GetAsync(e.Author.Id);
            var inventory = await new UserInventory(discordClientWrapper, apiClient, randomizer).Create(e.Author.Id, member);

            var loadingEmbed = await inventory.CreateLoadingInventoryEmbed();
            var message = await discordClientWrapper.Messages.RespondAsync(e.Message, loadingEmbed);
            inventory.BindToMessage(message);
            userInventoryService.Create(message.Id, inventory);

            var embed = inventory.UpdateEmbedForCurrentPage();
            await discordClientWrapper.Messages.ModifyAsync(message, embed);

            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("arrow_left"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("arrow_right"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("one"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("two"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("three"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("four"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("five"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("repeat"));
            await discordClientWrapper.Emotes.AddReactionToMessageAsync(message, await discordClientWrapper.Emotes.FindEmoteAsync("x"));
        }

        public Task<string> Help()
        {
            return Task.FromResult("Получение списка всех предметов пользователя\nУправление:" +
                                   "Стрелочки - переключение страниц, все предметы разбиты постранично по типам" +
                                   "Выбор действия - сделать предмет активным или продать");
        }

        public string Name => "inventory";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IUserInventoryService userInventoryService;
        private readonly IRandomizer randomizer;
    }
}