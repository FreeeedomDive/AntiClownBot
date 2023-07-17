using AntiClown.Api.Dto.Economies;
using AntiClown.DiscordBot.Cache.Users;
using AntiClown.DiscordBot.Extensions;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Transactions;

public class TransactionsEmbedBuilder : ITransactionsEmbedBuilder
{
    public TransactionsEmbedBuilder(IUsersCache usersCache)
    {
        this.usersCache = usersCache;
    }

    public async Task<DiscordEmbed> Build(Guid userId, TransactionDto[] transactions)
    {
        var messageEmbedBuilder = new DiscordEmbedBuilder();
        var member = await usersCache.GetMemberByApiIdAsync(userId);
        messageEmbedBuilder.WithTitle($"Транзакции {member.ServerOrUserName()}");
        if (member is not null)
        {
            messageEmbedBuilder.WithColor(member.Color);
            messageEmbedBuilder.WithThumbnail(member.AvatarUrl);
        }

        foreach (var transaction in transactions)
        {
            var sign = transaction.ScamCoinDiff > 0 ? "+" : "";
            messageEmbedBuilder.AddField(transaction.Reason, $"{sign}{transaction.ScamCoinDiff} скам-койнов ({transaction.DateTime.ToYekaterinburgTime()})");
        }

        return messageEmbedBuilder.Build();
    }

    private readonly IUsersCache usersCache;
}