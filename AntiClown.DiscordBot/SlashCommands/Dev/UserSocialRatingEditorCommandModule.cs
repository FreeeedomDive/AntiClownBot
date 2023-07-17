using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Dev;

[SlashCommandGroup(InteractionsIds.CommandsNames.Dev_UserEditor_Group, "Изменить что-то в рейтинге юзера", false), SlashCommandPermissions(Permissions.ViewAuditLog)]
public class UserSocialRatingEditorCommandModule : SlashCommandModuleWithMiddlewares
{
    public UserSocialRatingEditorCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_UserEditor_Coins, "Добавить / убавить скам-койнов")]
    public async Task EditCoins(
        InteractionContext context,
        [Option("user", "Юзер")] DiscordUser user,
        [Option("diff", "Сколько добавить / убавить")]
        long diff,
        [Option("reason", "Причина")] string? reason = "На то воля админа"
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                await antiClownApiClient.Economy.UpdateScamCoinsAsync(userId, (int)diff, reason!);
                await RespondToInteractionAsync(context, "done");
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Dev_UserEditor_LootBox, "Выдать или забрать лутбокс")]
    public async Task EditLootBoxes(
        InteractionContext context,
        [Option("user", "Юзер")] DiscordUser user,
        [Option("operation", "Выдать или забрать")]
        LootboxOperation operation
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(user.Id);
                switch (operation)
                {
                    case LootboxOperation.Give:
                        await antiClownApiClient.Economy.UpdateLootBoxesAsync(userId, 1);
                        break;
                    case LootboxOperation.Remove:
                        await antiClownApiClient.Economy.UpdateLootBoxesAsync(userId, -1);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
                }

                await RespondToInteractionAsync(context, "done");
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IUsersCache usersCache;
}