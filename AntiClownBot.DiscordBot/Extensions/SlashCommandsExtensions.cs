using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.Extensions;

public static class SlashCommandsExtensions
{
    public static void RegisterCommands<T>(this SlashCommandsExtension slash, T slashCommandModule, ulong guildId)
        where T : ApplicationCommandModule
    {
        slash.RegisterCommands<T>(guildId);
    }
}