using AntiClownApiClient.Dto.Models.Items;
using AntiClownApiClient.Dto.Responses;

namespace AntiClownApiClient.Dto.Responses.UserCommandResponses
{
    public class ItemResponseDto: BaseResponseDto
    {
        public ItemResult Result { get; set; }
        public BaseItem Item { get; set; }
    }

    public enum ItemResult
    {
        Success,
        ItemNotFound
    }
}