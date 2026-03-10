using AntiClown.Data.Api.Dto.Rights;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

[SlashCommandGroup(InteractionsIds.CommandsNames.Members_Admin_Group, "Управление участниками", false)]
[SlashCommandPermissions(Permissions.ViewAuditLog)]
public class AdminMembersCommandModule(
    IUsersCache usersCache,
    ICommandExecutor commandExecutor
) : SlashCommandModuleWithMiddlewares(commandExecutor)
{
    [SlashCommand(InteractionsIds.CommandsNames.Members_Admin_GetApiId, "Получить API ID по Discord пользователю")]
    public async Task GetApiIdByMember(
        InteractionContext context,
        [Option("member", "Discord пользователь")] DiscordUser member
    )
    {
        await ExecuteEphemeralWithRightsAsync(
            context, async () =>
            {
                var apiId = await usersCache.GetApiIdByMemberIdAsync(member.Id);
                var embed = new DiscordEmbedBuilder()
                            .WithTitle("API ID пользователя")
                            .WithThumbnail(member.AvatarUrl)
                            .AddField("Discord", $"{member.Username} ({member.Id})")
                            .AddField("API ID", apiId.ToString())
                            .Build();
                await RespondToInteractionAsync(context, embed);
            }, RightsDto.EditSettings
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Members_Admin_GetMember, "Получить Discord пользователя по API ID")]
    public async Task GetMemberByApiId(
        InteractionContext context,
        [Option("apiId", "API ID пользователя")] string apiId
    )
    {
        await ExecuteEphemeralWithRightsAsync(
            context, async () =>
            {
                if (!Guid.TryParse(apiId, out var userId))
                {
                    await RespondToInteractionAsync(context, $"Некорректный формат ID: `{apiId}`");
                    return;
                }

                var discordMember = await usersCache.GetMemberByApiIdAsync(userId);
                if (discordMember is null)
                {
                    await RespondToInteractionAsync(context, $"Пользователь с API ID `{userId}` не найден");
                    return;
                }

                var embed = new DiscordEmbedBuilder()
                            .WithTitle("Discord пользователь")
                            .WithThumbnail(discordMember.AvatarUrl)
                            .AddField("API ID", userId.ToString())
                            .AddField("Discord ID", discordMember.Id.ToString())
                            .AddField("Username", discordMember.Username)
                            .AddField("Nickname", discordMember.Nickname ?? "—")
                            .Build();
                await RespondToInteractionAsync(context, embed);
            }, RightsDto.EditSettings
        );
    }
}
