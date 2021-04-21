using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.GamblingCommands
{
    public class CurrentGambleCommand: BaseCommand
    {
        public CurrentGambleCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            await e.Message.RespondAsync(Config.CurrentGamble != null ? Config.CurrentGamble.ToString() : "В данный момент нет активной ставки");
        }

        public override string Help()
        {
            return "Получение текущей активной ставки";
        }
    }
}