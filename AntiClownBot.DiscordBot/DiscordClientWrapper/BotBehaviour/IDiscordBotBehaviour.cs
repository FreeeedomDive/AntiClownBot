using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.DiscordClientWrapper.BotBehaviour;

public interface IDiscordBotBehaviour
{
    Task ConfigureAsync(ApplicationCommandModule[] slashCommandModules);
}