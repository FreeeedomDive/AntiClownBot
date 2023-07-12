using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Core.Transactions.Services;
using AntiClown.Api.Dto.Economies;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/economy")]
public class TransactionsController : Controller
{
    public TransactionsController(
        ITransactionsService transactionsService,
        IMapper mapper
    )
    {
        this.transactionsService = transactionsService;
        this.mapper = mapper;
    }

    [HttpGet("{userId:guid}/transactions")]
    public async Task<ActionResult<TransactionDto[]>> ReadTransactions([FromRoute] Guid userId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var transactions = await transactionsService.ReadManyAsync(userId, skip, take);
        return mapper.Map<TransactionDto[]>(transactions);
    }

    [HttpPost("transactions/find")]
    public async Task<ActionResult<TransactionDto[]>> FindTransactions([FromBody] TransactionsFilterDto filter)
    {
        var transactions = await transactionsService.FindAsync(mapper.Map<TransactionsFilter>(filter));
        return mapper.Map<TransactionDto[]>(transactions);
    }

    private readonly ITransactionsService transactionsService;
    private readonly IMapper mapper;
}