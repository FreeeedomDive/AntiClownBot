using AntiClown.Api.Dto.Economies;

namespace AntiClown.Api.Client.Economy;

public interface ITransactionsClient
{
    Task<TransactionDto[]> ReadAsync(Guid userId, int skip = 0, int take = 10);
}