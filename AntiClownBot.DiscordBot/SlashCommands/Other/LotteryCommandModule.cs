using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Emotes;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class LotteryCommandModule : SlashCommandModuleWithMiddlewares
{
    public LotteryCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IEmotesProvider emotesProvider
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesProvider = emotesProvider;
    }

    [SlashCommand(Interactions.Commands.Lottery, "Информация о лотерее")]
    public async Task LotteryInfo(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            var emotes = Models
                .Lottery
                .Lottery
                .GetAllEmotes()
                .OrderBy(Models.Lottery.Lottery.EmoteToInt)
                .ToArray();
            var discordEmotesTasks = emotes
                .Select(emote => emotesProvider.GetEmoteAsTextAsync(emote.ToString()));
            var discordEmotes = await Task.WhenAll(discordEmotesTasks);
            var emotesPrice = emotes.Select(Models.Lottery.Lottery.EmoteToInt).ToArray();
            var emotesInfo = discordEmotes.Select((x, i) => $"\t{x} = {emotesPrice[i]}");
            var response = ">>> Информация о лотерее\n" +
                           "Смайлики, которые могут выпасть:\n" +
                           string.Join("\n", emotesInfo) +
                           "\nКоличество одинаковых смайликов тоже влияет на результат\n" +
                           "1 смайлик = x1, 2 смайлика = x5, 3 смайлика = x10, 4 смайлика = x50, 5 смайликов = x100";

            await RespondToInteractionAsync(context, response);
        });
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesProvider emotesProvider;
}