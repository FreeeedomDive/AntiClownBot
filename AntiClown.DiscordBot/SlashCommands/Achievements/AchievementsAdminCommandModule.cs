using AntiClown.Api.Client;
using AntiClown.Api.Dto.Achievements;
using AntiClown.Data.Api.Dto.Rights;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Achievements;

[SlashCommandGroup(InteractionsIds.CommandsNames.Achievements_Admin_Group, "Управление достижениями", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class AchievementsAdminCommandModule : SlashCommandModuleWithMiddlewares
{
    public AchievementsAdminCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache,
        IHttpClientFactory httpClientFactory
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.usersCache = usersCache;
        this.httpClientFactory = httpClientFactory;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Achievements_Admin_List, "Список ачивок")]
    public async Task ListAchievements(InteractionContext context)
    {
        await ExecuteEphemeralWithRightsAsync(
            context, async () =>
            {
                var achievements = await antiClownApiClient.Achievements.ReadAllAsync();
                await RespondToInteractionAsync(
                    context,
                    achievements.Length > 0
                        ? string.Join(
                            '\n',
                            achievements.Select(x => $"{x.Id}: {x.Name}")
                        )
                        : "Не создано ни одной ачивки"
                );
            },
            RightsDto.EditSettings
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Achievements_Admin_Create, "Создать новую ачивку")]
    public async Task CreateAchievement(
        InteractionContext context,
        [Option("name", "Название")] string name,
        [Option("logo", "Логотип (PNG)")] DiscordAttachment? logo = null
    )
    {
        await ExecuteEphemeralWithRightsAsync(
            context, async () =>
            {
                if (logo is null)
                {
                    await RespondToInteractionAsync(context, "А где логотип??");
                    return;
                }

                var fileName = logo.FileName ?? string.Empty;
                if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    await RespondToInteractionAsync(context, $"Логотип должен быть в формате PNG, а ты прикрепил {fileName}");
                    return;
                }

                using var httpClient = httpClientFactory.CreateClient();
                var imageBytes = await httpClient.GetByteArrayAsync(logo.Url);
                var logoBase64 = Convert.ToBase64String(imageBytes);

                var achievementId = await antiClownApiClient.Achievements.CreateAsync(
                    new NewAchievementDto
                    {
                        Name = name,
                        Logo = logoBase64,
                    }
                );

                await RespondToInteractionAsync(context, $"ID: {achievementId}");
            },
            RightsDto.EditSettings
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Achievements_Admin_Grant, "Выдать достижение пользователю")]
    public async Task GrantAchievement(
        InteractionContext context,
        [Option("id", "ID достижения")] string achievementId,
        [Option("member", "Пользователь")] DiscordUser member
    )
    {
        await ExecuteEphemeralWithRightsAsync(
            context, async () =>
            {
                if (!Guid.TryParse(achievementId, out var achievementGuid))
                {
                    await RespondToInteractionAsync(context, $"Некорректный формат ID: {achievementId}");
                    return;
                }

                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(member.Id);

                await antiClownApiClient.Achievements.GrantAsync(
                    achievementGuid,
                    new GrantAchievementDto
                    {
                        UserId = apiUserId,
                        GrantedAt = DateTime.UtcNow,
                    }
                );

                await RespondToInteractionAsync(context, $"Достижение выдано пользователю {member.Mention}.");
            },
            RightsDto.EditSettings
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IUsersCache usersCache;
    private readonly IHttpClientFactory httpClientFactory;
}