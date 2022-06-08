using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Inventory;
using AntiClownDiscordBotVersion2.Models.Shop;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands
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

            var message = await discordClientWrapper.Messages.RespondAsync(context, null, InteractionResponseType.DeferredChannelMessageWithSource);
            userInventoryService.Create(context.Member.Id, inventory);

            var embed = inventory.UpdateEmbedForCurrentPage();
            var builder = new DiscordWebhookBuilder()
                .AddEmbed(embed)
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_one", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("one"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_two", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("two"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_three", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("three"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_four", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("four"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_five", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("five"))))
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_left", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("arrow_left"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_right", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("arrow_right"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_repeat", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("repeat"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "inventory_x", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("x"))));
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

            await discordClientWrapper.Messages.RespondAsync(context, embedBuilder.Build());
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
            var message = await discordClientWrapper.Messages.RespondAsync(context, null, InteractionResponseType.DeferredChannelMessageWithSource);
            shopService.Create(context.Member.Id, shop);

            var embed = await shop.GetNewShopEmbed();
            var builder = new DiscordWebhookBuilder()
                .AddEmbed(embed)
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_one", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("one"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_two", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("two"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_three", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("three"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_four", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("four"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_five", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("five"))))
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_COGGERS", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_pepeSearching", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("pepeSearching"))));
            await discordClientWrapper.Messages.ModifyEmbedAsync(context, builder);
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IUserInventoryService userInventoryService;
        private readonly IRandomizer randomizer;
        private readonly IShopService shopService;
    }
}
