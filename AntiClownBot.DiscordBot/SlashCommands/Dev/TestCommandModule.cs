using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

[SlashCommandGroup("test", "Тестовая команда")]
public class TestCommandModule : SlashCommandModuleWithMiddlewares
{
    private readonly IDiscordClientWrapper discordClientWrapper;

    public TestCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
    }

    [SlashCommand("common", "Обычная команда")]
    public async Task TestCommon(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await discordClientWrapper.Messages.ModifyAsync(context, "да");
        });
    }

    [SlashCommand("ephemeral", "Эфемерная команда")]
    public async Task TestEphemeral(InteractionContext context)
    {
        await ExecuteEphemeralAsync(context, async () =>
        {
            await discordClientWrapper.Messages.ModifyAsync(context, "эфемерное да");
        });
    }
}