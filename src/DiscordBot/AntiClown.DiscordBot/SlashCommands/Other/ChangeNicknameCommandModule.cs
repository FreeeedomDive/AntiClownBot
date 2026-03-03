using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.DiscordBot.Cache.Emotes;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.DiscordClientWrapper;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.Other;

public class ChangeNicknameCommandModule : SlashCommandModuleWithMiddlewares
{
    public ChangeNicknameCommandModule(
        ICommandExecutor commandExecutor,
        IDiscordClientWrapper discordClientWrapper,
        IEmotesCache emotesCache,
        IAntiClownApiClient antiClownApiClient,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.discordClientWrapper = discordClientWrapper;
        this.emotesCache = emotesCache;
        this.antiClownApiClient = antiClownApiClient;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.ChangeNickname, "Изменить никнейм себе или другому челу")]
    public async Task ChangeNickname(
        InteractionContext context,
        [Option("member", "Чел, которому хочешь поменять никнейм")]
        DiscordUser userToEdit,
        [Option("newName", "Новое имя")] string newName
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var apiUserId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var selfChanging = context.Member.Id == userToEdit.Id;
                var cost = selfChanging ? -1000 : -2000;
                var balance = (await antiClownApiClient.Economy.ReadAsync(apiUserId)).ScamCoins;
                if (balance < -cost)
                {
                    await RespondToInteractionAsync(
                        context,
                        "Недостаточно скам койнов для совершения операции"
                    );
                    return;
                }

                try
                {
                    var member = await discordClientWrapper.Members.GetAsync(userToEdit.Id);
                    await discordClientWrapper.Members.ModifyAsync(member, model => { model.Nickname = newName; });
                    await antiClownApiClient.Economy.UpdateScamCoinsAsync(apiUserId, new UpdateScamCoinsDto
                        {
                            UserId = apiUserId,
                            Reason = $"Изменение никнейма пользователю {newName}",
                            ScamCoinsDiff = cost,
                        });
                    await RespondToInteractionAsync(
                        context,
                        await emotesCache.GetEmoteAsTextAsync("YEP")
                    );
                }
                catch
                {
                    await RespondToInteractionAsync(context, "Произошла неведомая хуйня");
                }
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IDiscordClientWrapper discordClientWrapper;
    private readonly IEmotesCache emotesCache;
    private readonly IUsersCache usersCache;
}