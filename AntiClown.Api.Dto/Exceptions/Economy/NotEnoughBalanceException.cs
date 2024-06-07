using Xdd.HttpHelpers.Models.Exceptions;

namespace AntiClown.Api.Dto.Exceptions.Economy;

public class NotEnoughBalanceException : ConflictException
{
    public NotEnoughBalanceException(Guid userId, int userBalance, int operationCost)
        : base($"User {userId} had {userBalance} balance, but required {operationCost}")
    {
        UserId = userId;
        UserBalance = userBalance;
        OperationCost = operationCost;
    }
    
    public Guid UserId { get; set; }
    public int UserBalance { get; set; }
    public int OperationCost { get; set; }
}