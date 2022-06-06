using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.ShopResponses
{
    public class ShopItemRevealResponseDto: BaseResponseDto
    {
        public Enums.RevealResult RevealResult { get; set; }
        public ShopItemDto Item { get; set; }
    }
}