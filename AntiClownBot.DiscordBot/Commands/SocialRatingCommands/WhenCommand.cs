using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using AntiClownDiscordBotVersion2.Utils.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownDiscordBotVersion2.Commands.SocialRatingCommands
{
    [ObsoleteCommand("when")]
    public class WhenCommand : ICommand
    {
        public WhenCommand(
            IDiscordClientWrapper discordClientWrapper,
            IApiClient apiClient,
            IGuildSettingsService guildSettingsService,
            IRandomizer randomizer
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.apiClient = apiClient;
            this.guildSettingsService = guildSettingsService;
            this.randomizer = randomizer;
        }

        public async Task Execute(MessageCreateEventArgs e)
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            if (e.Channel.Id != guildSettings.TributeChannelId && e.Channel.Id != guildSettings.HiddenTestChannelId)
            {
                var madgeEmote = await discordClientWrapper.Emotes.FindEmoteAsync("Madge");
                var pointRightEmote = await discordClientWrapper.Emotes.FindEmoteAsync("point_right");
                var tributeChannel = await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.TributeChannelId);
                await discordClientWrapper.Messages.RespondAsync(e.Message, $"{madgeEmote} {pointRightEmote} {tributeChannel.Mention}");
                return;
            }

            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.WithTitle("А когда же подношение???");

            var result = await apiClient.Users.WhenNextTributeAsync(e.Author.Id);
            var cooldownHasPassed = DateTime.Now > result.NextTribute;

            if (cooldownHasPassed)
            {
                embedBuilder.WithColor(DiscordColor.Green);
                embedBuilder.AddField(
                    "Уже пора!!!",
                    $"Срочно нужно исполнить партийный долг {(await discordClientWrapper.Emotes.FindEmoteAsync("flag_cn")).ToString().Multiply(3)}"
                );
                embedBuilder.AddField($"А мог бы прийти и пораньше {await discordClientWrapper.Emotes.FindEmoteAsync("Clueless")}",
                    $"Ты опоздал на {Utility.GetTimeDiff(result.NextTribute)}");
                await discordClientWrapper.Messages.RespondAsync(e.Message, embedBuilder.Build());
                return;
            }

            embedBuilder.WithColor(DiscordColor.Red);
            var roflan = randomizer.GetRandomNumberBetween(0, 100);
            if (roflan == 69)
            {
                var aRolf = $"{await discordClientWrapper.Emotes.FindEmoteAsync("aRolf")}".Multiply(5);
                embedBuilder.AddField("Завтра в 3", aRolf);
            }
            else
            {
                embedBuilder.AddField($"А подношение император XI через {Utility.GetTimeDiff(result.NextTribute)}",
                    $"Приходи не раньше чем {Utility.NormalizeTime(result.NextTribute)}");
            }

            await discordClientWrapper.Messages.RespondAsync(e.Message, embedBuilder.Build());
        }

        public Task<string> Help()
        {
            return Task.FromResult("Позволяет узнать время следующего подношения императору");
        }

        public string Name => "when";
        public bool IsObsolete => false;

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IApiClient apiClient;
        private readonly IGuildSettingsService guildSettingsService;
        private readonly IRandomizer randomizer;
    }
}