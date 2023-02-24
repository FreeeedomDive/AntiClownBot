using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class LotteryCommandModule : ApplicationCommandModule
{
    public LotteryCommandModule(IDiscordClientWrapper discordClientWrapper)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    [SlashCommand("lottery", "Информация о лотерее")]
    public async Task LotteryInfo(InteractionContext context)
    {
        var emotes = Models
            .Lottery
            .Lottery
            .GetAllEmotes()
            .OrderBy(Models.Lottery.Lottery.EmoteToInt)
            .ToArray();
        var discordEmotesTasks = emotes
            .Select(emote => discordClientWrapper.Emotes.FindEmoteAsync(emote.ToString()));
        var discordEmotes = await Task.WhenAll(discordEmotesTasks);
        var emotesPrice = emotes.Select(Models.Lottery.Lottery.EmoteToInt).ToArray();
        var emotesInfo = discordEmotes.Select((x, i) => $"\t{x} = {emotesPrice[i]}");
        var response = ">>> Информация о лотерее\n" +
                       "Смайлики, которые могут выпасть:\n" +
                       string.Join("\n", emotesInfo) +
                       "\nКоличество одинаковых смайликов тоже влияет на результат\n" +
                       "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 смайликов = x100";

        await discordClientWrapper.Messages.RespondAsync(context, response);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
}