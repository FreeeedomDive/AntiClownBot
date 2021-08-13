using System.Collections.Generic;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
{
    public class UserShopDto: BaseResponseDto
    {
        public List<ShopItemDto> Items = new();
    }
}