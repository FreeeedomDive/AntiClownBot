using AntiClown.Api.Core.Shops.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/shops")]
public class ShopController : Controller
{
    public ShopController(
        IShopsService shopsService,
        IMapper mapper
    )
    {
        this.shopsService = shopsService;
        this.mapper = mapper;
    }

    private readonly IShopsService shopsService;
    private readonly IMapper mapper;
}