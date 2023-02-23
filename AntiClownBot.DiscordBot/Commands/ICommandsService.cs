using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands;

public interface ICommandsService
{
    void UseCommands(Dictionary<string, ICommand> commands);
    bool TryGetCommand(string name, out ICommand? command);
    Task ExecuteCommand(string name, MessageCreateEventArgs e);
    IEnumerable<string> GetAllCommandNames();
}