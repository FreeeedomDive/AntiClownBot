using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Base;

public class SlashCommandContext
{
    public InteractionContext Context { get; init; }
    public SlashCommandOptions Options { get; init; }
}