using AntiClown.Api.Core.Economies.Services;
using AntiClown.Api.Core.Inventory.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AntiClown.Api.Controllers;

[Route("api/[controller]")]
public class InventoryController : Controller
{
    public InventoryController(
        IItemsService itemsService,
        IMapper mapper
    )
    {
        this.itemsService = itemsService;
        this.mapper = mapper;
    }

    private readonly IItemsService itemsService;
    private readonly IMapper mapper;
}