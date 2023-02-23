using DSharpPlus.SlashCommands;
using TelemetryApp.Api.Client.Log;

namespace AntiClownDiscordBotVersion2.Extensions;

public static class SlashCommandsExtensions
{
    public static void RegisterCommands<T>(this SlashCommandsExtension slash, T slashCommandModule, ulong guildId, ILoggerClient? logger = null)
        where T : ApplicationCommandModule
    {
        var type = typeof(T);
        slash.RegisterCommands(typeof(T), guildId);
        logger?.DebugAsync("Registered slash command module: {Type}", type).GetAwaiter().GetResult();
    }
}