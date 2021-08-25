using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.StatsCommands
{
    public class EmojiStatsCommand: BaseCommand
    {
        public EmojiStatsCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            await e.Message.RespondAsync(Config.GetEmojiStats());
        }

        public override string Help()
        {
            return "Статистика использованных на сервере эмоджи";
        }
    }
}