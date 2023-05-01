using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.SlashCommands.Base;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class ChangeNicknameCommandModule : SlashCommandModuleWithMiddlewares
{
    public ChangeNicknameCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IUserBalanceService userBalanceService
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.apiClient = apiClient;
        this.userBalanceService = userBalanceService;
    }

    [SlashCommand("nickname", "Изменить никнейм себе или другому челу")]
    public async Task ChangeNickname(
        InteractionContext context,
        [Option("member", "Чел, которому хочешь поменять никнейм")]
        DiscordUser userToEdit,
        [Option("newName", "Новое имя")] string newName
    )
    {
        await ExecuteAsync(context, async () =>
        {
            var userId = context.User.Id;
            var selfChanging = userId == userToEdit.Id;
            var cost = selfChanging ? -1000 : -2000;
            var balance = (await apiClient.Users.RatingAsync(userId)).ScamCoins;
            if (balance < -cost)
            {
                await RespondToInteractionAsync(
                    context,
                    "Недостаточно скам койнов для совершения операции"
                );
                return;
            }

            var member = await discordClientWrapper.Members.GetAsync(userToEdit.Id);

            try
            {
                await discordClientWrapper.Members.ModifyAsync(member, model => { model.Nickname = newName; });
                await userBalanceService.ChangeUserBalanceWithDailyStatsAsync(userId, cost,
                    $"Изменение никнейма пользователю {member.Id}");
                await RespondToInteractionAsync(
                    context,
                    $"{await discordClientWrapper.Emotes.FindEmoteAsync("YEP")}"
                );
            }
            catch
            {
                await RespondToInteractionAsync(context, "Произошла неведомая хуйня");
            }
        });
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
}