/* Generated file */
namespace AntiClown.Api.Client.Transactions;

public interface ITransactionsClient
{
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TransactionDto[]> ReadTransactionsAsync(System.Guid userId, System.Int32 skip, System.Int32 take);
    System.Threading.Tasks.Task<AntiClown.Api.Dto.Economies.TransactionDto[]> FindTransactionsAsync(AntiClown.Api.Dto.Economies.TransactionsFilterDto filter);
}
