using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Random;

public class RollCommandModule : SlashCommandModuleWithMiddlewares
{
    public RollCommandModule(
        ICommandExecutor commandExecutor,
        IRandomizer randomizer
    ) : base(commandExecutor)
    {
        this.randomizer = randomizer;
    }

    [SlashCommand(Interactions.Commands.Roll, "Рандомное число от a до b включительно")]
    public async Task Roll(
        InteractionContext context,
        [Option("a", "a")] long a,
        [Option("b", "b")] long b
    )
    {
        await ExecuteAsync(context, async () =>
        {
            await RespondToInteractionAsync(
                context,
                randomizer.GetRandomNumberBetweenIncludeRange(a, b).ToString()
            );
        });
    }

    private readonly IRandomizer randomizer;
}