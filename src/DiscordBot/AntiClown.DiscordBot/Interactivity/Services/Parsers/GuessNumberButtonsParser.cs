using System.Text.RegularExpressions;
using AntiClown.DiscordBot.Models.Interactions;

namespace AntiClown.DiscordBot.Interactivity.Services.Parsers;

public static class GuessNumberButtonsParser
{
    public static (Guid Id, int Pick) Parse(string buttonId)
    {
        const string pattern = $"{InteractionsIds.EventsButtons.GuessNumber.Prefix}_(.*)_(.*)";
        var match = new Regex(pattern).Matches(buttonId).First();
        var eventId = Guid.Parse(match.Groups[1].Value);
        var pick = int.Parse(match.Groups[2].Value);
        return (eventId, pick);
    }
}