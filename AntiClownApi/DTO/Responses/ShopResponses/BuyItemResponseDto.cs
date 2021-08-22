using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database;

namespace AntiClownBotApi.DTO.Responses.ShopResponses
{
    public class BuyItemResponseDto: BaseResponseDto
    {
        public Enums.BuyResult BuyResult { get; set; }
        public Guid ItemId { get; set; }
    }
}