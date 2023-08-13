using System.Text.RegularExpressions;
using AntiClown.DiscordBot.Models.Interactions;

namespace AntiClown.DiscordBot.Interactivity.Services.Parsers;

public static class ShopButtonsParser
{
    public static (Guid Id, string Action) Parse(string buttonId)
    {
        const string pattern = $"{InteractionsIds.ShopButtons.Prefix}_(.*)_(.*)";
        var match = new Regex(pattern).Matches(buttonId).First();
        var inventoryId = Guid.Parse(match.Groups[1].Value);
        var action = match.Groups[2].Value;
        return (inventoryId, action);
    }
}