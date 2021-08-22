using AntiClownBotApi.Constants;
using AntiClownBotApi.Database;

namespace AntiClownBotApi.DTO.Responses.ShopResponses
{
    public class ShopItemRevealResponseDto: BaseResponseDto
    {
        public Enums.RevealResult RevealResult { get; set; }
        public ShopItemDto Item { get; set; }
    }
}