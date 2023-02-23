using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Random;

public class SelectCommandModule : ApplicationCommandModule
{
    public SelectCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IRandomizer randomizer,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
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
        var lines = options.Split("//");
        if (lines.Length < 2)
        {
            await discordClientWrapper.Messages.RespondAsync(context, "Вариантов выбора должно быть 2 и более");
            return;
        }

        var selected = randomizer.GetRandomNumberBetween(1, lines.Length);
        await discordClientWrapper.Messages.RespondAsync(context, lines[selected]);
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IRandomizer randomizer;
    private readonly IGuildSettingsService guildSettingsService;
}