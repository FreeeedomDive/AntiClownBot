using AntiClownApiClient;
using AntiClownDiscordBotVersion2.DiscordClientWrapper;
using AntiClownDiscordBotVersion2.UserBalance;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClownDiscordBotVersion2.SlashCommands.Other;

public class ChangeNicknameCommandModule : ApplicationCommandModule
{
    public ChangeNicknameCommandModule(
        IDiscordClientWrapper discordClientWrapper,
        IApiClient apiClient,
        IUserBalanceService userBalanceService
    )
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
        var userId = context.User.Id;
        var selfChanging = userId == userToEdit.Id;
        var cost = selfChanging ? -1000 : -2000;
        var balance = (await apiClient.Users.RatingAsync(userId)).ScamCoins;
        if (balance < -cost)
        {
            await discordClientWrapper.Messages.RespondAsync(
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
        }
        catch
        {
            await discordClientWrapper.Messages.RespondAsync(context, "Произошла неведомая хуйня");
            return;
        }

        await discordClientWrapper.Messages.RespondAsync(
            context,
            $"{await discordClientWrapper.Emotes.FindEmoteAsync("YEP")}"
        );
    }

    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IApiClient apiClient;
    private readonly IUserBalanceService userBalanceService;
}