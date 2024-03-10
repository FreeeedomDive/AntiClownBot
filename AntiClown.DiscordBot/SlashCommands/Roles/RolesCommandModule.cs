using AntiClown.Api.Client;
using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Client.Extensions;
using AntiClown.Data.Api.Dto.Rights;
using AntiClown.Data.Api.Dto.Settings;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.Roles.Repositories;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Roles;

[SlashCommandGroup(InteractionsIds.CommandsNames.Roles_Group, "Управление ролями")]
public class RolesCommandModule : SlashCommandModuleWithMiddlewares
{
    public RolesCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IAntiClownApiClient antiClownApiClient,
        IRolesRepository rolesRepository,
        IUsersCache usersCache,
        IAntiClownDataApiClient antiClownDataApiClient
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.antiClownApiClient = antiClownApiClient;
        this.rolesRepository = rolesRepository;
        this.usersCache = usersCache;
        this.antiClownDataApiClient = antiClownDataApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Roles_All, "Посмотреть доступные роли")]
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

    [SlashCommand(InteractionsIds.CommandsNames.Roles_New, "Создать новую роль и получить ее себе")]
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

            var createRolePrice = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.DiscordGuild, "CreateRolePrice");
            var userId = await usersCache.GetApiIdByMemberIdAsync(context.User.Id);
            var economy = await antiClownApiClient.Economy.ReadAsync(userId);

            if (economy.ScamCoins < createRolePrice)
            {
                await RespondToInteractionAsync(
                    context,
                    $"Для создания новой роли нужно отдать {createRolePrice} скамкойнов, тебе не хватает {createRolePrice - economy.ScamCoins}"
                );
                return;
            }

            var newRole = await discordClientWrapper.Roles.CreateNewRoleAsync(name);
            await discordClientWrapper.Roles.GrantRoleAsync(context.User.Id, newRole);
            await RespondToInteractionAsync(context, $"Создал роль {newRole.Name} и добавил ее тебе");
            await antiClownApiClient.Economy.UpdateScamCoinsAsync(
                userId,
                -createRolePrice,
                $"Создание роли {name}");
            await rolesRepository.CreateAsync(newRole.Id);
        });
    }

    [SlashCommand(InteractionsIds.CommandsNames.Roles_Grant, "Получить роль")]
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

            var joinRolePrice = await antiClownDataApiClient.Settings.ReadAsync<int>(SettingsCategory.DiscordGuild, "JoinRolePrice");
            var userId = await usersCache.GetApiIdByMemberIdAsync(context.User.Id);
            var economy = await antiClownApiClient.Economy.ReadAsync(userId);
            var serverMember = await discordClientWrapper.Members.GetAsync(context.User.Id);

            if (serverMember.Roles.Any(x => x.Id == role.Id))
            {
                await RespondToInteractionAsync(
                    context,
                    $"У тебя уже есть роль {role.Name}"
                );
                return;
            }
        
            if (economy.ScamCoins < joinRolePrice)
            {
                await RespondToInteractionAsync(
                    context,
                    $"Для получения роли нужно отдать {joinRolePrice} скамкойнов, тебе не хватает {joinRolePrice - economy.ScamCoins}"
                );
                return;
            }

            await discordClientWrapper.Roles.GrantRoleAsync(context.User.Id, role);
            await RespondToInteractionAsync(context, $"Выдал роль {role.Name}");
            await antiClownApiClient.Economy.UpdateScamCoinsAsync(userId, -joinRolePrice, $"Получение роли {role.Name}");

            var f1RoleId = await antiClownDataApiClient.Settings.ReadAsync<ulong>(SettingsCategory.DiscordGuild, "F1RoleId");
            if (role.Id == f1RoleId)
            {
                await antiClownDataApiClient.Rights.GrantAsync(userId, RightsDto.F1Predictions);
            }
        });
    }

    [SlashCommand(InteractionsIds.CommandsNames.Roles_Revoke, "Убрать роль")]
    public async Task RevokeRoleAsync(
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
        var rolesToJoin = await rolesRepository.ReadAllAsync();
        return rolesToJoin
               .Where(id => serverRoles.TryGetValue(id, out var _))
               .Select(id => serverRoles[id])
               .ToArray();
    }

    private static string JoinOrLeaveRoleHelp() => "Чтобы посмотреть все роли, которые можно получить или убрать, введи /roles -a";

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IRolesRepository rolesRepository;
    private readonly IUsersCache usersCache;
    private readonly IAntiClownDataApiClient antiClownDataApiClient;
}