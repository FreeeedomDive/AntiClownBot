using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.Party;
using AntiClownDiscordBotVersion2.Settings.GuildSettings;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Roles;

[SlashCommandGroup("roles", "Управление ролями")]
public class RolesCommandModule : SlashCommandModuleWithMiddlewares
{
    public RolesCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IUserBalanceService userBalanceService,
        IGuildSettingsService guildSettingsService,
        IPartyService partyService
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.userBalanceService = userBalanceService;
        this.guildSettingsService = guildSettingsService;
        this.partyService = partyService;
    }

    [SlashCommand("-a", "Посмотреть доступные роли")]
    public async Task ListRolesAsync(
        InteractionContext context
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var joinableRoles = await GetAllRolesToJoinAsync();
            await RespondToInteractionAsync(
                context,
                $"Доступные роли:\n```\n{string.Join("\n", joinableRoles.Select(role => $"{role.Name}"))}\n```"
            );
        });
    }

    [SlashCommand("-n", "Создать новую роль и получить ее себе")]
    public async Task CreateRoleAsync(
        InteractionContext context,
        [Option("name", "Название новой роли")]
        string name
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var joinableRoles = await GetAllRolesToJoinAsync();
            if (joinableRoles.Any(x => x.Name == name))
            {
                await RespondToInteractionAsync(
                    context,
                    $"На сервере уже есть роль с названием {name}"
                );
                return;
            }

            var guildSettings = guildSettingsService.GetGuildSettings();
            var userId = context.User.Id;
            var apiUser = await apiClient.Users.RatingAsync(userId);

            if (apiUser.ScamCoins < guildSettings.CreateRolePrice)
            {
                await RespondToInteractionAsync(
                    context,
                    $"Для создания новой роли нужно отдать {guildSettings.CreateRolePrice} скамкойнов, тебе не хватает {guildSettings.CreateRolePrice - apiUser.ScamCoins}"
                );
                return;
            }

            var newRole = await discordClientWrapper.Roles.CreateNewRoleAsync(name);
            await discordClientWrapper.Roles.GrantRoleAsync(userId, newRole);
            await RespondToInteractionAsync(context, $"Создал роль {newRole.Name} и добавил ее тебе");
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(
                userId,
                -guildSettings.CreateRolePrice,
                $"Создание роли {name}");
            partyService.PartiesInfo.JoinableRoles.Add(newRole.Id);
            partyService.Save();
        });
    }

    [SlashCommand("-g", "Получить роль")]
    public async Task AddRoleToUser(
        InteractionContext context,
        [Option("role", "Роль")] DiscordRole role
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var joinableRoles = await GetAllRolesToJoinAsync();
            if (joinableRoles.All(x => x.Id != role.Id))
            {
                await RespondToInteractionAsync(
                    context,
                    $"Невозможно получить роль {role.Name}\n{JoinOrLeaveRoleHelp()}"
                );
                return;
            }

            var guildSettings = guildSettingsService.GetGuildSettings();
            var userId = context.User.Id;
            var userRating = await apiClient.Users.RatingAsync(userId);
            var serverMember = await discordClientWrapper.Members.GetAsync(userId);

            if (serverMember.Roles.Any(x => x.Id == role.Id))
            {
                await RespondToInteractionAsync(
                    context,
                    $"У тебя уже есть роль {role.Name}"
                );
                return;
            }
        
            if (userRating.ScamCoins < guildSettings.JoinRolePrice)
            {
                await RespondToInteractionAsync(
                    context,
                    $"Для получения роли нужно отдать {guildSettings.JoinRolePrice} скамкойнов, тебе не хватает {guildSettings.JoinRolePrice - userRating.ScamCoins}"
                );
                return;
            }

            await discordClientWrapper.Roles.GrantRoleAsync(userId, role);
            await RespondToInteractionAsync(context, $"Выдал роль {role.Name}");
            await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, -guildSettings.JoinRolePrice,
                $"Получение роли {role.Name}");
        });
    }

    [SlashCommand("-r", "Убрать роль")]
    public async Task LeaveRoleAsync(
        InteractionContext context,
        [Option("role", "Роль")] DiscordRole role
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var joinableRoles = await GetAllRolesToJoinAsync();
            if (joinableRoles.All(x => x.Id != role.Id))
            {
                await RespondToInteractionAsync(
                    context,
                    $"Невозможно убрать роль {role.Name}\n{JoinOrLeaveRoleHelp()}"
                );
                return;
            }

            var userId = context.User.Id;
            var serverMember = await discordClientWrapper.Members.GetAsync(userId);

            if (serverMember.Roles.All(x => x.Id != role.Id))
            {
                await RespondToInteractionAsync(
                    context,
                    $"У тебя и так нет роли {role.Name}"
                );
                return;
            }

            await discordClientWrapper.Roles.RevokeRoleAsync(userId, role);
            await RespondToInteractionAsync(context, $"Убрал роль {role.Name}");
        });
    }

    private async Task<DiscordRole[]> GetAllRolesToJoinAsync()
    {
        var guild = await discordClientWrapper.Guilds.GetGuildAsync();
        var serverRoles = guild.Roles;
        return partyService.PartiesInfo.JoinableRoles.Select(id => serverRoles[id]).ToArray();
    }

    private static string JoinOrLeaveRoleHelp() => "Чтобы посмотреть все роли, которые можно получить или убрать, введи /roles -a";

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
    private readonly IGuildSettingsService guildSettingsService;
    private readonly IPartyService partyService;
}