using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Models.Interactions;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.Utils;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Gaming
{
    [SlashCommandGroup(Interactions.Commands.Party_Group, "Собирайте пати в разные игры :)")]
    public class PartyCommandModule : SlashCommandModuleWithMiddlewares
    {
        public PartyCommandModule(
            ICommandExecutor commandExecutor,
            IDiscordClientWrapper discordClientWrapper,
            IPartyService partyService,
            IGuildSettingsService guildSettingsService
        ) : base(commandExecutor)
        {
            this.discordClientWrapper = discordClientWrapper;
            this.partyService = partyService;
            this.guildSettingsService = guildSettingsService;
        }

        [SlashCommand(Interactions.Commands.Party_CreateWithOldPrefix, "Быстрое создание пати по-старому")]
        public async Task CreateParty(
            InteractionContext context,
            [Option("game", "Короткое название игры")]
            PartyPrefix prefix,
            [Option("description", "Описание")] string? description = null
        )
        {
            await ExecuteEphemeralAsync(context, async () =>
            {
                var guildSettings = guildSettingsService.GetGuildSettings();

                if (!string.IsNullOrEmpty(description))
                {
                    description = $" - {description}";
                }

                var response = await discordClientWrapper.Emotes.FindEmoteAsync("Okayge");
                await RespondToInteractionAsync(context, response);
                switch (prefix)
                {
                    case PartyPrefix.Dota:
                        await partyService.CreateNewParty(context.Member.Id, $"Dota{description}", 5,
                            guildSettings.DotaRoleId);
                        break;
                    case PartyPrefix.CsGo:
                        await partyService.CreateNewParty(context.Member.Id, $"CS GO{description}", 5,
                            guildSettings.CsRoleId);
                        break;
                    case PartyPrefix.SiGame:
                        await partyService.CreateNewParty(context.Member.Id, $"ДЕРЖУ ИГРУ{description}", 7,
                            guildSettings.SiGameRoleId);
                        break;
                    default:
                        await discordClientWrapper.Messages.RespondAsync(
                            context,
                            $"Такую игру я не знаю {await discordClientWrapper.Emotes.FindEmoteAsync("CockInspector")}"
                        );
                        break;
                }
            });
        }

        [SlashCommand(Interactions.Commands.Party_Create, "Создать пати")]
        public async Task CreateParty(
            InteractionContext context,
            [Option("Name", "Название вашей группы")]
            string name,
            [Option("Role", "Если нужно пингануть какую-то роль")]
            DiscordRole? role = null,
            [Option("Capacity", "Ограничение на количество людей в группе. Дефолтно 5")]
            long capacity = 5)
        {
            await ExecuteEphemeralAsync(context, async () =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    await RespondToInteractionAsync(context,
                        $"Не вижу название игры {await discordClientWrapper.Emotes.FindEmoteAsync("modCheck")}");
                    return;
                }

                if (capacity is < 1 or > 20)
                {
                    await RespondToInteractionAsync(context,
                        $"Ага как скажешь {await discordClientWrapper.Emotes.FindEmoteAsync("Agakakskagesh")}");
                    return;
                }

                var response = await discordClientWrapper.Emotes.FindEmoteAsync("Okayge");
                await RespondToInteractionAsync(context, response);
                await partyService.CreateNewParty(context.Member.Id, name, (int)capacity, role?.Id);
            });
        }

        [SlashCommand(Interactions.Commands.Party_All, "Список текущих пати")]
        public async Task GetCurrentParties(InteractionContext context)
        {
            await ExecuteAsync(context, async () =>
            {
                var embed = await partyService.CreatePartyEmbed();
                await RespondToInteractionAsync(context, embed);
            });
        }

        [SlashCommand(Interactions.Commands.Party_Stats, "Статистика по времени сбора фулл пати")]
        public async Task GetStats(InteractionContext context)
        {
            await ExecuteAsync(context, async () =>
            {
                var partyStats = partyService.PartiesInfo.PartyStats;
                if (partyStats.TotalFullParties == 0)
                {
                    await RespondToInteractionAsync(context, "Не собрано ни одного пати для сбора статистики");
                    return;
                }

                var content =
                    $"Самое быстрое пати было собрано за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.FastestPartyInSeconds))}\n" +
                    $"В среднем пати собиралось за {Utility.GetTimeDiff(TimeSpan.FromSeconds(partyStats.TotalSeconds / partyStats.TotalFullParties))}";
                await RespondToInteractionAsync(context, content);
            });
        }

        private readonly IDiscordClientWrapper discordClientWrapper;
        private readonly IPartyService partyService;
        private readonly IGuildSettingsService guildSettingsService;
    }
}