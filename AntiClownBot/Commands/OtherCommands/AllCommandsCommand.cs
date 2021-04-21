using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.OtherCommands
{
    public class AllCommandsCommand : BaseCommand
    {
        private readonly CommandsManager _manager;

        public AllCommandsCommand(DiscordClient client, Configuration configuration, CommandsManager manager) : base(
            client, configuration)
        {
            _manager = manager;
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            await e.Message.RespondAsync($"Доступные команды:\n{string.Join("\n", _manager.AllCommands)}");
        }

        public override string Help()
        {
            return "Получение всех команд бота";
        }
    }
}