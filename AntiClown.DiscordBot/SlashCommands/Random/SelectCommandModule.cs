using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Tools.Utility.Random;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Random;

public class SelectCommandModule : SlashCommandModuleWithMiddlewares
{
    public SelectCommandModule(ICommandExecutor commandExecutor) : base(commandExecutor)
    {
    }

    [SlashCommand(InteractionsIds.CommandsNames.Select, "Если вы не можете решиться между несколькими вариантами, бот сделает это за вас")]
    public async Task Select(
        InteractionContext context,
        [Option("options", "Варианты, разделенные между собой символами // (минимум 2)")]
        string options
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var lines = options.Split("//");
                if (lines.Length < 2)
                {
                    await RespondToInteractionAsync(context, "Вариантов выбора должно быть 2 и более");
                    return;
                }

                var selected = Randomizer.GetRandomNumberBetween(1, lines.Length);
                await RespondToInteractionAsync(context, lines[selected]);
            }
        );
    }
}