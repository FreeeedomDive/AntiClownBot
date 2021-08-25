using System;

namespace AntiClownBotApi.DTO.Responses.ShopResponses
{
    public class ItemIdInSlotResponseDto: BaseResponseDto
    {
        public Guid ShopItemId { get; set; }
    }
}