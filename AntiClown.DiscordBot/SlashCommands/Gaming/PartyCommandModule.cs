using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.EmbedBuilders.Parties;
using AntiClown.DiscordBot.Extensions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using AntiClown.Entertainment.Api.Client;
using AntiClown.Entertainment.Api.Dto.Parties;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Gaming;

[SlashCommandGroup(InteractionsIds.CommandsNames.Party_Group, "Сбор пати в разные игры")]
public class PartyCommandModule : SlashCommandModuleWithMiddlewares
{
    public PartyCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient,
        IPartyEmbedBuilder partyEmbedBuilder,
        IEmotesCache emotesCache,
        IUsersCache usersCache,
        IAntiClownDataApiClient antiClownDataApiClient
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownEntertainmentApiClient = antiClownEntertainmentApiClient;
        this.partyEmbedBuilder = partyEmbedBuilder;
        this.emotesCache = emotesCache;
        this.usersCache = usersCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Party_CreateWithOldPrefix, "Быстрое создание пати по-старому")]
    public async Task CreateParty(
        InteractionContext context,
        [Option("game", "Короткое название игры")]
        PartyPrefix prefix,
        [Option("description", "Описание")] string description = ""
    )
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                var response = await emotesCache.GetEmoteAsTextAsync("Okayge");
                await RespondToInteractionAsync(context, $"{response}\n(Пати теперь создается асинхронно и появится в течение нескольких секунд)");
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                switch (prefix)
                {
                    case PartyPrefix.Dota:
                        await antiClownEntertainmentApiClient.Parties.CreateAsync(
                            new CreatePartyDto
                            {
                                CreatorId = userId,
                                MaxPlayers = 5,
                                Name = "Dota",
                                RoleId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "DotaRoleId"),
                                Description = description,
                            }
                        );
                        break;
                    case PartyPrefix.Cs2:
                        await antiClownEntertainmentApiClient.Parties.CreateAsync(
                            new CreatePartyDto
                            {
                                CreatorId = userId,
                                MaxPlayers = 5,
                                Name = "CS2",
                                RoleId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "CsRoleId"),
                                Description = description,
                            }
                        );
                        break;
                    case PartyPrefix.SiGame:
                        await antiClownEntertainmentApiClient.Parties.CreateAsync(
                            new CreatePartyDto
                            {
                                CreatorId = userId,
                                MaxPlayers = 7,
                                Name = "ДЕРЖУ ИГРУ",
                                RoleId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "SiGameRoleId"),
                                Description = description,
                            }
                        );
                        break;
                    default:
                        await discordClientWrapper.Messages.RespondAsync(
                            context,
                            $"Такую игру я не знаю {await emotesCache.GetEmoteAsTextAsync("CockInspector")}"
                        );
                        break;
                }
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Party_Create, "Создать пати")]
    public async Task CreateParty(
        InteractionContext context,
        [Option("Name", "Название вашей группы")]
        string name,
        [Option("Role", "Роль в дискорде")] DiscordRole role,
        [Option("Capacity", "Ограничение на количество людей в группе. Дефолтно 5")]
        long capacity = 5,
        [Option("Description", "Описание")] string description = ""
    )
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    await RespondToInteractionAsync(
                        context,
                        $"Не вижу название игры {await emotesCache.GetEmoteAsTextAsync("modCheck")}"
                    );
                    return;
                }

                if (capacity is < 1 or > 20)
                {
                    await RespondToInteractionAsync(
                        context,
                        $"Ага как скажешь {await emotesCache.GetEmoteAsTextAsync("Agakakskagesh")}"
                    );
                    return;
                }

                var response = await emotesCache.GetEmoteAsTextAsync("Okayge");
                await RespondToInteractionAsync(context, response);

                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                await antiClownEntertainmentApiClient.Parties.CreateAsync(
                    new CreatePartyDto
                    {
                        CreatorId = userId,
                        MaxPlayers = (int)capacity,
                        Name = name,
                        RoleId = role.Id,
                        Description = description,
                    }
                );
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Party_All, "Список текущих пати")]
    public async Task GetCurrentParties(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var openedParties = await antiClownEntertainmentApiClient.Parties.ReadOpenedAsync();
                var embed = await partyEmbedBuilder.BuildOpenedPartiesEmbedAsync(openedParties);
                await RespondToInteractionAsync(context, embed);
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Party_Stats, "Статистика по времени сбора фулл пати")]
    public async Task GetStats(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var fullParties = await antiClownEntertainmentApiClient.Parties.ReadFullAsync();
                if (fullParties.Length == 0)
                {
                    await RespondToInteractionAsync(context, "Полное пати еще ни разу не было собрано");
                    return;
                }

                var fullPartyTimesInSeconds = fullParties
                                              .Select(x => (x.FirstFullPartyAt! - x.CreatedAt).Value.TotalSeconds)
                                              .Order()
                                              .ToArray();
                var quickest = TimeSpan.FromSeconds(fullPartyTimesInSeconds.First()).ToTimeDiffString();
                var average = TimeSpan.FromSeconds(fullPartyTimesInSeconds.Sum() / fullParties.Length).ToTimeDiffString();
                await RespondToInteractionAsync(
                    context,
                    $"Самое быстрое пати было собрано за {quickest}\n" +
                    $"В среднем пати собиралось за {average}"
                );
            }
        );
    }

    private readonly IAntiClownEntertainmentApiClient antiClownEntertainmentApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IPartyEmbedBuilder partyEmbedBuilder;
    private readonly IUsersCache usersCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}