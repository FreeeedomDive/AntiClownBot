using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Log;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands;

public class CommandsService : ICommandsService
{
    public CommandsService(
        IDiscordClientWrapper discordClientWrapper,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService,
        ILogger logger
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
        this.logger = logger;
    }

    public void UseCommands(Dictionary<string, ICommand> commands)
    {
        this.commands = commands;
    }

    public bool TryGetCommand(string name, out ICommand command)
    {
        /*if (commands.ContainsKey(name))
        {
            command = commands[name];
            return true;
        }

        command = null;
        return false;*/
        command = commands.Values.FirstOrDefault(c => c.Name == name);
        return command != null;
    }

    public async Task ExecuteCommand(string name, MessageCreateEventArgs e)
    {
        var appSettings = appSettingsService.GetSettings();
        var guildSettings = guildSettingsService.GetGuildSettings();
        if (appSettings.MaintenanceMode && e.Author.Id != guildSettings.AdminId)
        {
            await discordClientWrapper.Messages.RespondAsync(
                e.Message,
                $"Пока не отвечаю {await discordClientWrapper.Emotes.FindEmoteAsync("NOPERS")}"
            );
            return;
        }

        logger.Info($"Загружено {commands.Count} команд");
        logger.Info(string.Join("\n", commands.Select(kv => $"|{kv.Key}| - {kv.Value}")));
        logger.Info($"{commands.ContainsKey(name)}");
        if (!TryGetCommand(name, out var command))
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, $"Нет команды с именем {name}");
            return;
        }

        await command.Execute(e);
    }

    public IEnumerable<string> GetAllCommandNames()
    {
        return commands.Keys.OrderBy(key => key);
    }

    private Dictionary<string, ICommand> commands;

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAppSettingsService appSettingsService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly ILogger logger;
}