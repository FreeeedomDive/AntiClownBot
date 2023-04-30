using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

[SlashCommandGroup("test", "Тестовая команда")]
public class TestCommandModule : SlashCommandModuleWithMiddlewares
{
    public TestCommandModule(
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
        
    }

    [SlashCommand("common", "Обычная команда")]
    public async Task TestCommon(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("да"));
        });
    }

    [SlashCommand("ephemeral", "Эфемерная команда")]
    public async Task TestEphemeral(InteractionContext context)
    {
        await ExecuteEphemeralAsync(context, async () =>
        {
            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("эфемерное да"));
        });
    }
}