using AntiClown.Api.Core.Transactions.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/[controller]")]
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

    private readonly ITransactionsService transactionsService;
    private readonly IMapper mapper;
}