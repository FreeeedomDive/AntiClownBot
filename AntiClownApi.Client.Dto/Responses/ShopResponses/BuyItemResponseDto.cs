using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.ShopResponses
{
    public class BuyItemResponseDto: BaseResponseDto
    {
        public Enums.BuyResult BuyResult { get; set; }
        public Guid ItemId { get; set; }
    }
}