/* Generated file */
using System.Threading.Tasks;

namespace AntiClown.Api.Client.Transactions;

public interface ITransactionsClient
{
    Task<AntiClown.Api.Dto.Economies.TransactionDto[]> ReadTransactionsAsync(System.Guid userId, int skip = 0, int take = 10);
    Task<AntiClown.Api.Dto.Economies.TransactionDto[]> FindTransactionsAsync(AntiClown.Api.Dto.Economies.TransactionsFilterDto filter);
}
