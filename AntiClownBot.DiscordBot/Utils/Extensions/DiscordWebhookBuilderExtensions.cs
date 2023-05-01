using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Interactions;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Utils.Extensions
{
    public static class DiscordWebhookBuilderExtensions
    {
        public static async Task<DiscordWebhookBuilder> SetShopButtons(this DiscordWebhookBuilder builder, IDiscordClientWrapper discordClientWrapper)
        {
            return builder.AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem1, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("one"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem2, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("two"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem3, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("three"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem4, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("four"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem5, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("five"))))
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonReroll, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonChangeTool, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("pepeSearching"))));
        }

        public static async Task<DiscordWebhookBuilder> SetInventoryButtons(this DiscordWebhookBuilder builder, IDiscordClientWrapper discordClientWrapper)
        {
            return builder.AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton1, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("one"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton2, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("two"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton3, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("three"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton4, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("four"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton5, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("five"))))
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonLeft, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("arrow_left"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonRight, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("arrow_right"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonChangeActiveStatus, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("repeat"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonSell, null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("x"))));
        }
    }
}
