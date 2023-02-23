using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.AppSettings;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands;

public class CommandsService : ICommandsService
{
    public CommandsService(
        IDiscordClientWrapper discordClientWrapper,
        IAppSettingsService appSettingsService,
        IGuildSettingsService guildSettingsService
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.appSettingsService = appSettingsService;
        this.guildSettingsService = guildSettingsService;
    }

    public void UseCommands(Dictionary<string, ICommand> commands)
    {
        this.commands = commands;
    }

    public bool TryGetCommand(string name, out ICommand? command)
    {
        return commands.TryGetValue(name, out command);
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

        if (!TryGetCommand(name, out var command) && command is not null)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message, $"Нет команды с именем {name}");
            return;
        }

        var attribute = Attribute.GetCustomAttribute(command!.GetType(), typeof(ObsoleteCommandAttribute));
        if (attribute is ObsoleteCommandAttribute obsoleteCommandAttribute)
        {
            await discordClientWrapper.Messages.RespondAsync(e.Message,
                $"Команда {name} устарела, воспользуйся аналогичной слеш-командой {obsoleteCommandAttribute.SlashCommand}");
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
}