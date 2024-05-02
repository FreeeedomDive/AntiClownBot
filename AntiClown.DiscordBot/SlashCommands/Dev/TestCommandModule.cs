using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.DiscordBot.Utility.Locks;
using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

public class TestCommandModule : SlashCommandModuleWithMiddlewares
{
    public TestCommandModule(ICommandExecutor commandExecutor, ILocker locker) : base(commandExecutor)
    {
        this.locker = locker;
    }

    [SlashCommand("test", "test", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
    public async Task Test(InteractionContext context)
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                await locker.DoInLockAsync(
                    "TestLock", async () =>
                    {
                        await Task.Delay(TimeSpan.FromSeconds(30));
                        await RespondToInteractionAsync(context, "Released lock");
                    }
                );
            }
        );
    }

    private readonly ILocker locker;
}