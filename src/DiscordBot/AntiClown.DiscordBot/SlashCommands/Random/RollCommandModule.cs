using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Tools.Utility.Random;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Random;

public class RollCommandModule : SlashCommandModuleWithMiddlewares
{
    public RollCommandModule(ICommandExecutor commandExecutor) : base(commandExecutor)
    {
    }

    [SlashCommand(InteractionsIds.CommandsNames.Roll, "Рандомное число от a до b включительно")]
    public async Task Roll(
        InteractionContext context,
        [Option("a", "a")] long a,
        [Option("b", "b")] long b
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                await RespondToInteractionAsync(
                    context,
                    Randomizer.GetRandomNumberInclusive((int)a, (int)b).ToString()
                );
            }
        );
    }
}