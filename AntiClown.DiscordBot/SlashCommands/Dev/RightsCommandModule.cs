using AntiClown.Data.Api.Client;
using AntiClown.Data.Api.Dto.Rights;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

[SlashCommandGroup(InteractionsIds.CommandsNames.Dev_Rights_Group, "Права", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class RightsCommandModule : SlashCommandModuleWithMiddlewares
{
    public RightsCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownDataApiClient antiClownDataApiClient,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownDataApiClient = antiClownDataApiClient;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_Rights_All, "Показать все права")]
    public async Task ShowAllRights(InteractionContext context)
    {
        await ExecuteWithRightsAsync(
            context, async () =>
            {
                var allRights = await antiClownDataApiClient.Rights.ReadAllAsync();
                var embedBuilder = new DiscordEmbedBuilder();
                embedBuilder.AddField("Права", string.Join("\n", allRights.Keys), true);
                embedBuilder.AddField("Количество пользователей", string.Join("\n", allRights.Values.Select(x => x.Length)), true);
                await RespondToInteractionAsync(context, embedBuilder.Build());
            },
            RightsDto.EditRights
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_Rights_Show, "Показать права пользователя")]
    public async Task ShowUserRights(InteractionContext context, [Option("user", "Пользователь")] DiscordUser user)
    {
        await ExecuteWithRightsAsync(
            context, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                var rights = await antiClownDataApiClient.Rights.FindAllUserRightsAsync(apiUserId);
                await RespondToInteractionAsync(context, rights.Length == 0 ? "У этого юзера нет прав" : string.Join("\n", rights));
            },
            RightsDto.EditRights
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_Rights_Grant, "Выдать права пользователю")]
    public async Task GrantRights(
        InteractionContext context,
        [Option("user", "Пользователь")] DiscordUser user,
        [Option("rights", "Права")] RightsDto rightsDto
    )
    {
        await ExecuteWithRightsAsync(
            context, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                await antiClownDataApiClient.Rights.GrantAsync(apiUserId, rightsDto);
            },
            RightsDto.EditRights
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_Rights_Revoke, "Отобрать права у пользователя")]
    public async Task RevokeRights(
        InteractionContext context,
        [Option("user", "Пользователь")] DiscordUser user,
        [Option("rights", "Права")] RightsDto rightsDto
    )
    {
        await ExecuteWithRightsAsync(
            context, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                await antiClownDataApiClient.Rights.GrantAsync(apiUserId, rightsDto);
            },
            RightsDto.EditRights
        );
    }

    private readonly IAntiClownDataApiClient antiClownDataApiClient;
    private readonly IUsersCache usersCache;
}