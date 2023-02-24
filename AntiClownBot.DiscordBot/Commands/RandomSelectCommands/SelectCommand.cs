using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.RandomSelectCommands
{
    [ObsoleteCommand("select")]
    public class SelectCommand : ICommand
    {
        public SelectCommand(
            IDiscordClientWrapper discordClientWrapper,
            IRandomizer randomizer,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.randomizer = randomizer;
            this.guildSettingsService = guildSettingsService;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var message = e.Message.Content;
            var lines = message.Split('\n');
            if (lines.Length < 2)
            {
                await discordClientWrapper.Messages.RespondAsync(e.Message, "Вариантов выбора должно быть 2 и более");
                return;
            }

            var selected = randomizer.GetRandomNumberBetween(1, lines.Length);
            await discordClientWrapper.Messages.RespondAsync(e.Message,lines[selected]);
        }

        public Task<string> Help()
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            var result = "Если вы не можете решиться между несколькими вариантами, бот сделает это за вас\n" +
                   "Использование:" +
                   $"\n{guildSettings.CommandsPrefix}{Name}\n[Вариант 1]\n[Вариант 2]\n...\n[Вариант N]";
            
            return Task.FromResult(result);
        }

        public string Name => "select";

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IRandomizer randomizer;
        private readonly IGuildSettingsService guildSettingsService;
    }
}