using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands
{
    public class HelpCommand: BaseCommand
    {
        private readonly CommandsManager _manager; 
        public HelpCommand(DiscordClient client, Configuration configuration, CommandsManager manager) : base(client, configuration)
        {
            _manager = manager;
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            if (messageArgs.Length < 2)
            {
                await e.Message.RespondAsync("Не обнаружена команда");
                return;
            }
            var commandString = $"!{messageArgs[1]}";
            if (!_manager.GetCommandByName(commandString, out var command))
            {
                await e.Message.RespondAsync($"Не существует команды с именем {commandString}");
                return;
            }

            await e.Message.RespondAsync(command.Help());
        }

        public override string Help()
        {
            return "Получение справки о команде бота\nИспользование:\n!help [команда]";
        }
    }
}