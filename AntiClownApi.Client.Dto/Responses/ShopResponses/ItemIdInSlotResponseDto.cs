using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.ShopResponses
{
    public class ItemIdInSlotResponseDto: BaseResponseDto
    {
        public Guid ShopItemId { get; set; }
    }
}