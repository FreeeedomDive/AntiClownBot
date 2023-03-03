using AntiClown.Api.Dto.Exceptions.Base;

namespace AntiClown.Api.Dto.Exceptions.Economy;

public class NotEnoughBalanceException : AntiClownApiConflictException
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