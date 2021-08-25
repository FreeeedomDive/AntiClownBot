using System;

namespace ApiWrapper.Responses.ShopResponses
{
    public class BuyItemResponseDto: BaseResponseDto
    {
        public Enums.BuyResult BuyResult { get; set; }
        public Guid ItemId { get; set; }
    }
}