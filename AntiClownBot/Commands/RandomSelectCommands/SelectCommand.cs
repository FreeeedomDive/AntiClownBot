using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.RandomSelectCommands
{
    public class SelectCommand: BaseCommand
    {
        public SelectCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e, SocialRatingUser user)
        {
            var message = e.Message.Content;
            var lines = message.Split('\n');
            if (lines.Length < 2)
            {
                await e.Message.RespondAsync("Вариантов выбора должно быть 2 и более");
                return;
            }
            var selected = Randomizer.GetRandomNumberBetween(1, lines.Length);
            await e.Message.RespondAsync(lines[selected]);
        }

        public override string Help()
        {
            return "Если вы не можете решиться между несколькими вариантами, бот сделает это за вас\n" +
                   "Использование:\n!select\n[Вариант 1]\n[Вариант 2]\n...\n[Вариант N]";
        }
    }
}