using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public class SlashCommandContext
{
    public InteractionContext Context { get; init; }
    public SlashCommandOptions Options { get; init; }
    public bool IsRejected { get; private set; }

    public void Reject()
    {
        IsRejected = true;
    }
}
