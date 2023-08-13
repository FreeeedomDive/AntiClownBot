namespace AntiClown.Api.Core.Transactions.Domain;

public class TransactionsFilter
{
    public Guid? UserId { get; set; }
    public DateTimeRange? DateTimeRange { get; set; }
}