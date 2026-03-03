using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Base;

public class SlashCommandContext
{
    public InteractionContext Context { get; init; }
    public SlashCommandOptions Options { get; init; }
}