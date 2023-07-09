using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Models.Interactions;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Utils.Extensions
{
    public static class DiscordWebhookBuilderExtensions
    {
        public static async Task<DiscordWebhookBuilder> SetShopButtons(this DiscordWebhookBuilder builder,
            IEmotesProvider emotesProvider)
        {
            return builder
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem1, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("one"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem2, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("two"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem3, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("three"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem4, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("four"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonItem5, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("five"))))
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonReroll, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("COGGERS"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.ShopButtonChangeTool, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("pepeSearching"))));
        }

        public static async Task<DiscordWebhookBuilder> SetInventoryButtons(this DiscordWebhookBuilder builder,
            IEmotesProvider emotesProvider)
        {
            return builder.AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton1, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("one"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton2, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("two"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton3, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("three"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton4, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("four"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButton5, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("five"))))
                .AddComponents(
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonLeft, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("arrow_left"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonRight, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("arrow_right"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary,
                        Interactions.Buttons.InventoryButtonChangeActiveStatus, null, false,
                        new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("repeat"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, Interactions.Buttons.InventoryButtonSell, null,
                        false, new DiscordComponentEmoji(await emotesProvider.GetEmoteAsync("x"))));
        }
    }
}