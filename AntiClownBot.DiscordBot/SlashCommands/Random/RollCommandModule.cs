using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Random;

public class RollCommandModule : ApplicationCommandModule
{
    public RollCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IRandomizer randomizer
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.randomizer = randomizer;
    }

    [SlashCommand("roll", "Рандомное число от a до b включительно")]
    public async Task Roll(
        InteractionContext context,
        [Option("a", "")] int a,
        [Option("b", "")] int b
    )
    {
        if (a > b)
        {
            (a, b) = (b, a);
        }

        await discordClientWrapper.Messages.RespondAsync(context,
            $"{randomizer.GetRandomNumberBetweenIncludeRange(a, b)}"
        );
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IRandomizer randomizer;
}