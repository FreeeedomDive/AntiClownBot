using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Responses.UserCommandResponses;
using AntiClownBotApi.Models.Items;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/users/{userId}/items")]
    public class ItemsController : Controller
    {
        private ItemRepository ItemRepository { get; }

        public ItemsController(ItemRepository itemRepository)
        {
            ItemRepository = itemRepository;
        }

        [HttpGet("")]
        public List<BaseItem> AllItems(ulong userId) =>
            ItemRepository.GetUserItems(userId).Select(BaseItem.FromDbItem).ToList();

        [HttpPost("{itemId:guid}/active/{isActive:bool}")]
        public SetActiveStatusForItemResponseDto SetActiveStatusForItem(ulong userId, Guid itemId, bool isActive) =>
            new()
            {
                Result = ItemRepository.SetActiveStatusForItem(userId, itemId, isActive)
            };

        [HttpPost("{itemId:guid}/sell")]
        public SellItemResponseDto SellItem(ulong userId, Guid itemId) =>
            new()
            {
                Result = ItemRepository.SellItem(userId, itemId)
            };
    }
}