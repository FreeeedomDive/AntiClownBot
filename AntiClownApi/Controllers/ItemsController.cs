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
        private GlobalState GlobalState { get; }

        public ItemsController(ItemRepository itemRepository, GlobalState globalState)
        {
            ItemRepository = itemRepository;
            GlobalState = globalState;
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

        [HttpPost("lootbox/open")]
        public OpenLootBoxResultDto OpenLootBox(ulong userId)
        {
            var lootBoxResult = ItemRepository.TryOpenLootBox(userId, out var reward);
            var result = new OpenLootBoxResultDto()
            {
                UserId = userId,
                IsSuccessful = lootBoxResult,
                Reward = reward
            };

            return result;
        }
        
        [HttpPost("lootbox/add")]
        public void AddLootBoxes(ulong userId)
        {
            GlobalState.GiveLootBoxToUser(userId);
        }
        
        [HttpPost("lootbox/remove")]
        public void RemoveLootBoxes(ulong userId)
        {
            GlobalState.RemoveLootBoxFromUser(userId);
        }
    }
}