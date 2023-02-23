using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Dev;

[SlashCommandGroup("userEditor", "Изменить что-то в рейтинге юзера", false)]
[SlashCommandPermissions(Permissions.Administrator)]
public class UserSocialRatingEditorCommandModule : ApplicationCommandModule
{
    public UserSocialRatingEditorCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IUserBalanceService userBalanceService,
        IApiClient apiClient
    )
    {
        this.discordClientWrapper = discordClientWrapper;
        this.userBalanceService = userBalanceService;
        this.apiClient = apiClient;
    }

    [SlashCommand("coins", "Добавить / убавить скам-койнов")]
    public async Task EditCoins(
        InteractionContext context,
        [Option("user", "Юзер")] DiscordUser user,
        [Option("diff", "Сколько добавить / убавить")] long diff,
        [Option("reason", "Причина")] string? reason = "На то воля админа"
    )
    {
        await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(user.Id, (int)diff, reason!);
        await discordClientWrapper.Messages.RespondAsync(context, "done");
    }

    [SlashCommand("lootbox", "Выдать или забрать лутбокс")]
    public async Task EditLootboxes(
        InteractionContext context,
        [Option("user", "Юзер")] DiscordUser user,
        [Option("operation", "Выдать или забрать")] LootboxOperation operation
    )
    {
        switch (operation)
        {
            case LootboxOperation.Give:
                await apiClient.Items.AddLootBoxAsync(user.Id);
                break;
            case LootboxOperation.Remove:
                await apiClient.Items.RemoveLootBoxAsync(user.Id);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
        }

        await discordClientWrapper.Messages.RespondAsync(context, "done");
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IUserBalanceService userBalanceService;
    private readonly IApiClient apiClient;
}