using AntiClownDiscordBotVersion2.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

public class TestCommandModule : SlashCommandModuleWithMiddlewares
{
    public TestCommandModule(
        ICommandExecutor commandExecutor
    ) : base(commandExecutor)
    {
        
    }

    [SlashCommand("test", "тест")]
    public async Task Test(InteractionContext context)
    {
        await ExecuteAsync(context, async () =>
        {
            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("да"));
        });
    }
}