using AntiClown.Api.Core.Economies.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/[controller]")]
public class EconomyController : Controller
{
    public EconomyController(
        IEconomyService economyService,
        IMapper mapper
    )
    {
        this.economyService = economyService;
        this.mapper = mapper;
    }

    private readonly IEconomyService economyService;
    private readonly IMapper mapper;
}