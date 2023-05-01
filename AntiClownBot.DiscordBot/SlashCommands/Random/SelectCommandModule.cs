using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Random;

public class SelectCommandModule : SlashCommandModuleWithMiddlewares
{
    public SelectCommandModule(
        ICommandExecutor commandExecutor,
        IRandomizer randomizer,
        IGuildSettingsService guildSettingsService
    ) : base(commandExecutor)
    {
        this.randomizer = randomizer;
        this.guildSettingsService = guildSettingsService;
    }

    [SlashCommand("select", "Если вы не можете решиться между несколькими вариантами, бот сделает это за вас")]
    public async Task Select(
        InteractionContext context,
        [Option("options", "Варианты, разделенные между собой символами // (минимум 2)")]
        string options
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var lines = options.Split("//");
            if (lines.Length < 2)
            {
                await RespondToInteractionAsync(context, "Вариантов выбора должно быть 2 и более");
                return;
            }

            var selected = randomizer.GetRandomNumberBetween(1, lines.Length);
            await RespondToInteractionAsync(context, lines[selected]);
        });
    }

    private readonly IRandomizer randomizer;
    private readonly IGuildSettingsService guildSettingsService;
}