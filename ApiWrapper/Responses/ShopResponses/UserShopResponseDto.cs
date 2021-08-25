using System.Collections.Generic;

namespace ApiWrapper.Responses.ShopResponses
{
    public class UserShopResponseDto: BaseResponseDto
    {
        public int Balance { get; set; }
        public int ReRollPrice { get; set; }
        public int FreeItemReveals { get; set; }
        public List<ShopItemDto> Items { get; set; }
    }
}