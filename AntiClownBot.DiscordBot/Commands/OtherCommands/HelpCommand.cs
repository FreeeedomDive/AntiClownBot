using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.OtherCommands
{
    public class HelpCommand : ICommand
    {
        private readonly IDiscordClientWrapper discordClientWrapper;

        public HelpCommand(
            IDiscordClientWrapper discordClientWrapper,
            ICommandsService commandsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.commandsService = commandsService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            if (messageArgs.Length < 2)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    e.Message,
                    $"Доступные команды:\n{string.Join("\n", commandsService.GetAllCommandNames())}"
                );
                return;
            }

            var commandName = messageArgs[1];
            if (!commandsService.TryGetCommand(commandName, out var command) && command is not null)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"Не существует команды с именем {commandName}");
                return;
            }

            await discordClientWrapper.Messages.RespondAsync(e.Message, await command!.Help());
        }

        public Task<string> Help()
        {
            return Task.FromResult("Получение справки о команде бота\nИспользование:\n!help [команда]");
        }

        public string Name => "help";
        public bool IsObsolete => false;

        private readonly ICommandsService commandsService;
    }
}