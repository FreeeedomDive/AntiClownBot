using System;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class ShopItemDto: BaseResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsRevealed { get; set; }
    }
}