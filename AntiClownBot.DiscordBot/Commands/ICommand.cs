using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands;

public interface ICommand
{
    Task Execute(MessageCreateEventArgs e);
    Task<string> Help();

    string Name { get; }
    bool IsObsolete { get; }
}