using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.Commands.SlashCommands
{
    [SlashCommandGroup("party", "Собирайте пати в разные игры :)")]
    public class PartyCommandModule : ApplicationCommandModule
    {
        public PartyCommandModule(
            IDiscordClientWrapper discordClientWrapper,
            IPartyService partyService,
            IGuildSettingsService guildSettingsService
        )
        {
            this.discordClientWrapper = discordClientWrapper;
            this.partyService = partyService;
            this.guildSettingsService = guildSettingsService;
        }

        [SlashCommand("create", "Создать пати")]
        public async Task CreateParty(
            InteractionContext context,
            [Option("Name", "Название вашей группы")] string name,
            [Option("Role", "Если нужно пингануть какую-то роль")] DiscordRole? role = null,
            [Option("Capacity", "Ограничение на количество людей в группе. Дефолтно 5")] long capacity = 5)
        {
            var guildSettings = guildSettingsService.GetGuildSettings();
            if (context.Channel.Id != guildSettings.PartyChannelId && context.Channel.Id != guildSettings.HiddenTestChannelId)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    context,
                    $"{await discordClientWrapper.Emotes.FindEmoteAsync("Madge")} " +
                    $"{await discordClientWrapper.Emotes.FindEmoteAsync("point_right")} " +
                    $"{(await discordClientWrapper.Guilds.FindDiscordChannel(guildSettings.PartyChannelId)).Mention}"
                );
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                await discordClientWrapper.Messages.RespondAsync(
                    context,
                    $"Не вижу название игры {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}"
                );
                return;
            }

            if (capacity is < 1 or > 20)
            {
                await discordClientWrapper.Messages.RespondAsync(
                    context,
                    $"Ага как скажешь {await discordClientWrapper.Emotes.FindEmoteAsync("Agakakskagesh")}"
                );
                return;
            }

            await partyService.CreateNewParty(context.Member.Id, name, (int)capacity, role?.Id);
        }

        [SlashCommand("list", "Текущие пати")]
        public async Task GetCurrentParties(InteractionContext context)
        {
            await partyService.AddPartyObserverMessage(context);
        }

        [SlashCommand("stats", "Статистика по времени сбора фулл пати")]
        public async Task GetStats(InteractionContext context)
        {
            var partyStats = partyService.PartiesInfo.PartyStats;
            if (partyStats.TotalFullParties == 0)
            {
                await discordClientWrapper.Messages.RespondAsync(context, "Не собрано ни одного пати для сбора статистики");
                return;
            }

            var content = $"Самое быстрое пати было собрано за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.FastestPartyInSeconds))}\n" +
                          $"В среднем пати собиралось за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.TotalSeconds / partyStats.TotalFullParties))}";
            await discordClientWrapper.Messages.RespondAsync(context, content);
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IPartyService partyService;
        private readonly IGuildSettingsService guildSettingsService;
    }
}
