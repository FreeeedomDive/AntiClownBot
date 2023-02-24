using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.RandomSelectCommands
{
    [ObsoleteCommand("roll")]
    public class RollCommand : ICommand
    {
        public RollCommand(
            IDiscordClientWrapper discordClientWrapper,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.randomizer = randomizer;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            var messageArgs = message.Split(' ');
            if (messageArgs.Length != 3)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "Хуевый запрос, чел");
                return;
            }

            if (!int.TryParse(messageArgs[1], out var a) || !int.TryParse(messageArgs[2], out var b))
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message,"Хуевый запрос, чел");
                return;
            }

            if (a > b)
            {
                (a, b) = (b, a);
            }

            await discordClientWrapper.Messages.RespondAsync(e.Message,$"{randomizer.GetRandomNumberBetweenIncludeRange(a, b)}");
        }

        public Task<string> Help()
        {
            return Task.FromResult("Получение рандомного числа в диапазоне от a до b включительно");
        }

        public string Name => "roll";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IRandomizer randomizer;
    }
}