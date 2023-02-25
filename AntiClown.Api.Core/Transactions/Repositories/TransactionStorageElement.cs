using Microsoft.EntityFrameworkCore;
using SqlRepositoryBase.Core.Models;

namespace AntiClown.Api.Core.Transactions.Repositories;

[Index("UserId", "DateTime")]
public class TransactionStorageElement : SqlStorageElement
{
    public Guid UserId { get; set; }
    public int ScamCoinDiff { get; set; }
    public DateTime DateTime { get; set; }
    public string Reason { get; set; }
}