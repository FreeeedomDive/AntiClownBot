using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Inventory
{
    public class InventoryCommandModule : ApplicationCommandModule
    {
        public InventoryCommandModule(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IUserInventoryService userInventoryService,
            IShopService shopService,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.userInventoryService = userInventoryService;
            this.randomizer = randomizer;
            this.shopService = shopService;
        }

        [SlashCommand("inventory", "Посмотреть свой инветарь")]
        public async Task GetInventory(InteractionContext context)
        {
            var member = await discordClientWrapper.Members.GetAsync(context.Member.Id);
            var inventory = await new UserInventory(discordClientWrapper, apiClient, randomizer).Create(context.Member.Id, member);

            await discordClientWrapper.Messages.RespondAsync(context, null, InteractionResponseType.DeferredChannelMessageWithSource);
            userInventoryService.Create(context.Member.Id, inventory);

            var embed = inventory.UpdateEmbedForCurrentPage();
            var builder = await new DiscordWebhookBuilder()
                .AddEmbed(embed)
                .SetInventoryButtons(discordClientWrapper);
            await discordClientWrapper.Messages.ModifyEmbedAsync(context, builder);
        }

        [SlashCommand("lootbox", "Открыть лутбокс")]
        public async Task OpenLootbox(InteractionContext context)
        {
            var result = await apiClient.Items.OpenLootBoxAsync(context.Member.Id);
            if (!result.IsSuccessful)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    context,
                    $"Нет доступных лутбоксов {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}"
                );
                return;
            }

            await discordClientWrapper.Messages.RespondAsync(context, null, InteractionResponseType.DeferredChannelMessageWithSource);

            var member = await discordClientWrapper.Members.GetAsync(context.Member.Id);
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle($"{member.ServerOrUserName()} открывает лутбокс...");
            embedBuilder.WithColor(member.Color);

            var reward = result.Reward;
            embedBuilder.AddField("Денежное вознаграждение", $"Ты получил {reward.ScamCoinsReward} scam coins!");

            var itemIndex = 1;
            foreach (var item in reward.Items)
            {
                embedBuilder.AddField($"{itemIndex++} предмет", $"Ты получил {item.Rarity} {item.Name}\n" +
                                                                string.Join("\n",
                                                                    item
                                                                        .Description()
                                                                        .Select(kv => $"{kv.Key}: {kv.Value}")));
            }

            await discordClientWrapper.Messages.ModifyEmbedAsync(context, new DiscordWebhookBuilder().AddEmbed(embedBuilder.Build()));
        }

        [SlashCommand("shop", "Посмотреть свой магазин")]
        public async Task GetShop(InteractionContext context)
        {
            var member = await discordClientWrapper.Members.GetAsync(context.Member.Id);
            var shop = new Shop(discordClientWrapper, apiClient, randomizer)
            {
                UserId = context.Member.Id,
                Member = member
            };
            await discordClientWrapper.Messages.RespondAsync(context, null, InteractionResponseType.DeferredChannelMessageWithSource);
            shopService.Create(context.Member.Id, shop);

            var embed = await shop.GetNewShopEmbed();
            var builder = await new DiscordWebhookBuilder()
                .AddEmbed(embed)
                .SetShopButtons(discordClientWrapper);
            await discordClientWrapper.Messages.ModifyEmbedAsync(context, builder);
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IUserInventoryService userInventoryService;
        private readonly IRandomizer randomizer;
        private readonly IShopService shopService;
    }
}
