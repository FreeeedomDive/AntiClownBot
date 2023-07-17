using AntiClown.Api.Dto.Economies;
using DSharpPlus.Entities;

namespace AntiClown.DiscordBot.EmbedBuilders.Transactions;

public interface ITransactionsEmbedBuilder
{
    Task<DiscordEmbed> Build(Guid userId, TransactionDto[] transactions);
}