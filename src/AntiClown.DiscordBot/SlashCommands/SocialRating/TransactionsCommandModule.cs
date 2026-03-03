using AntiClown.Api.Client;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.EmbedBuilders.Transactions;
using AntiClown.DiscordBot.Models.Interactions;
using AntiClown.DiscordBot.SlashCommands.Base;
using DSharpPlus.SlashCommands;

namespace AntiClown.DiscordBot.SlashCommands.SocialRating;

public class TransactionsCommandModule : SlashCommandModuleWithMiddlewares
{
    public TransactionsCommandModule(
        ICommandExecutor commandExecutor,
        IAntiClownApiClient antiClownApiClient,
        ITransactionsEmbedBuilder transactionsEmbedBuilder,
        IUsersCache usersCache
    ) : base(commandExecutor)
    {
        this.antiClownApiClient = antiClownApiClient;
        this.transactionsEmbedBuilder = transactionsEmbedBuilder;
        this.usersCache = usersCache;
    }

    [SlashCommand(InteractionsIds.CommandsNames.Transactions, "Посмотреть свои транзакции")]
    public async Task ShowTransactions(
        InteractionContext context
    )
    {
        await ExecuteAsync(
            context, async () =>
            {
                var userId = await usersCache.GetApiIdByMemberIdAsync(context.Member.Id);
                var transactions = await antiClownApiClient.Transactions.ReadTransactionsAsync(userId);
                var embed = await transactionsEmbedBuilder.Build(userId, transactions);
                await RespondToInteractionAsync(context, embed);
            }
        );
    }

    private readonly IAntiClownApiClient antiClownApiClient;
    private readonly ITransactionsEmbedBuilder transactionsEmbedBuilder;
    private readonly IUsersCache usersCache;
}