using System;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database;
using AntiClownBotApi.Database.DBControllers;
using AntiClownBotApi.DTO.Responses.ShopResponses;
using Microsoft.AspNetCore.Mvc;

namespace AntiClownBotApi.Controllers
{
    [ApiController]
    [Route("/api/shop/{userId}")]
    public class ShopController : Controller
    {
        [HttpGet, Route("")]
        public UserShopResponseDto GetUserShop(ulong userId)
        {
            var user = ShopDbController.GetUserShop(userId);
            return new UserShopResponseDto()
            {
                UserId = userId,
                Balance = user.Economy.ScamCoins,
                FreeItemReveals = user.Shop.FreeItemReveals,
                ReRollPrice = user.Shop.ReRollPrice,
                Items = user.Shop.Items.Select(dbItem => new ShopItemDto()
                {
                    Id = dbItem.Id,
                    Name = dbItem.Name,
                    Price = dbItem.Price,
                    Rarity = dbItem.Rarity,
                    IsOwned = dbItem.IsOwned,
                    IsRevealed = dbItem.IsRevealed
                }).ToList()
            };
        }

        [HttpPost, Route("getItemIdInSlot/{slot:int}")]
        public ItemIdInSlotResponseDto GetItemIdInSlot(ulong userId, int slot)
        {
            if (slot is <= 0 or > NumericConstants.MaximumItemsInShop)
            {
                return new ItemIdInSlotResponseDto()
                {
                    HasError = true,
                    Error = $"Номер слота должен быть от 1 до {NumericConstants.MaximumItemsInShop}"
                };
            }

            var result = ShopDbController.GetShopItemInSlot(userId, slot);

            return new ItemIdInSlotResponseDto
            {
                UserId = userId,
                ShopItemId = result
            };
        }

        [HttpPost, Route("reveal/{itemId:guid}")]
        public ShopItemRevealResponseDto Reveal(ulong userId, Guid itemId)
        {
            var result = ShopDbController.RevealItem(userId, itemId, out var revealedItem);

            return new ShopItemRevealResponseDto()
            {
                RevealResult = result,
                Item = revealedItem == null
                    ? null
                    : new ShopItemDto()
                    {
                        Id = revealedItem.Id,
                        Name = revealedItem.Name,
                        Price = revealedItem.Price,
                        Rarity = revealedItem.Rarity,
                        IsOwned = revealedItem.IsOwned,
                        IsRevealed = revealedItem.IsRevealed
                    }
            };
        }

        [HttpPost, Route("buy/{itemId:guid}")]
        public BuyItemResponseDto Buy(ulong userId, Guid itemId)
        {
            var result = ShopDbController.TryBuyItem(userId, itemId, out var item);
            var response = new BuyItemResponseDto() {UserId = userId, BuyResult = result};
            if (result == Enums.BuyResult.Success)
                response.ItemId = item.Id;

            return response;
        }

        [HttpPost, Route("reroll")]
        public ReRollResponseDto ReRoll(ulong userId) => new()
        {
            ReRollResult = ShopDbController.ReRollShop(userId)
        };
    }
}