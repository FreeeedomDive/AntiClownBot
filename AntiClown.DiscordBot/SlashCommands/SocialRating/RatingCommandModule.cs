using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Rating;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating;

public class RatingCommandModule : SlashCommandModuleWithMiddlewares
{
    public RatingCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        IRatingEmbedBuilder ratingEmbedBuilder,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.ratingEmbedBuilder = ratingEmbedBuilder;
        this.usersCache = usersCache;
        this.antiClownApiClient = antiClownApiClient;
    }

    [SlashCommand(InteractionsIds.CommandsNames.ScamCoins, "Узнать свой баланс скамкойнов (остальные его не увидят)")]
    public async Task FetchScamCoins(InteractionContext context)
    {
        await ExecuteEphemeralAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var userRating = await antiClownApiClient.Economy.ReadAsync(userId);

                await RespondToInteractionAsync(
                    context,
                    $"{userRating.ScamCoins} скам-койнов"
                );
            }
        );
    }

    [SlashCommand(InteractionsIds.CommandsNames.Rating, "Полный паспорт пользователя с информацией о балансе и активных предметах")]
    public async Task Rating(InteractionContext context)
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var economy = await antiClownApiClient.Economy.ReadAsync(userId);
                var inventory = await antiClownApiClient.Inventories.ReadInventoryAsync(userId);
                var embed = await ratingEmbedBuilder.BuildAsync(economy, inventory);

                await RespondToInteractionAsync(context, embed);
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly IRatingEmbedBuilder ratingEmbedBuilder;
    private readonly IUsersCache usersCache;
}