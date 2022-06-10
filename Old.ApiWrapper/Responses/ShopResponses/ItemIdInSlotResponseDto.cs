using System;

namespace ApiWrapper.Responses.ShopResponses
{
    public class ItemIdInSlotResponseDto: BaseResponseDto
    {
        public Guid ShopItemId { get; set; }
    }
}