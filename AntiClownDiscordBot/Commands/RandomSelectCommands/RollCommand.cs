using System;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace AntiClownBot.Commands.RandomSelectCommands
{
    public class RollCommand: BaseCommand
    {
        public RollCommand(DiscordClient client, Configuration configuration) : base(client, configuration)
        {
        }

        public override async void Execute(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            if (messageArgs.Length != 3)
            {
                await e.Message.RespondAsync("Хуевый запрос, чел");
                return;
            }

            if (!int.TryParse(messageArgs[1], out var a) || !int.TryParse(messageArgs[2], out var b))
            {
                await e.Message.RespondAsync("Хуевый запрос, чел");
                return;
            }

            if (a > b)
            {
                (a, b) = (b, a);
            }

            await e.Message.RespondAsync($"{Randomizer.GetRandomNumberBetweenIncludeRange(a, b)}");
        }

        public override string Help()
        {
            return "Получение рандомного числа в диапазоне от a до b включительно";
        }
    }
}