using AntiClown.Api.Client;
using AntiClown.Api.Dto.Economies;
using AntiClown.Web.Api.Attributes;
using AntiClown.Web.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Web.Api.Controllers;

[Route("webApi/economy/{userId:guid}")]
[RequireUserToken]
public class EconomyController : ControllerBase
{
    public EconomyController(IAntiClownApiClient antiClownApiClient)
    {
        this.antiClownApiClient = antiClownApiClient;
    }

    [HttpGet]
    public async Task<ActionResult<EconomyDto>> GetEconomy([FromRoute] Guid userId)
    {
        return await antiClownApiClient.Economy.ReadAsync(userId);
    }

    [HttpPost("transactions")]
    public async Task<ActionResult<TransactionDto[]>> GetTransactions([FromRoute] Guid userId, [FromBody] TransactionsFilter filter)
    {
        return await antiClownApiClient.Transactions.ReadTransactionsAsync(userId, filter.Offset, filter.Limit);
    }

    private readonly IAntiClownApiClient antiClownApiClient;
}