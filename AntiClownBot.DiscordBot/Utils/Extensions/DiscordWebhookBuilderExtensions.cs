using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus;
using DSharpPlus.Entities;

namespace AntiClownDiscordBotVersion2.Utils.Extensions
{
    public static class DiscordWebhookBuilderExtensions
    {
        public static async Task<DiscordWebhookBuilder> SetShopButtons(this DiscordWebhookBuilder builder, IDiscordClientWrapper discordClientWrapper)
        {
            return builder.AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_one", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("one"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_two", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("two"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_three", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("three"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_four", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("four"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_five", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("five"))))
                .AddComponents(
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_COGGERS", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("COGGERS"))),
                new DiscordButtonComponent(ButtonStyle.Secondary, "shop_pepeSearching", null, false, new DiscordComponentEmoji(await discordClientWrapper.Emotes.FindEmoteAsync("pepeSearching"))));
        }

        public static async Task<DiscordWebhookBuilder> SetInventoryButtons(this DiscordWebhookBuilder builder, IDiscordClientWrapper discordClientWrapper)
        {
            return builder.AddComponents(
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
        }
    }
}
