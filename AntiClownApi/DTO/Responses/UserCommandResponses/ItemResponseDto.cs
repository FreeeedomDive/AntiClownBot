using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.DTO.Responses.UserCommandResponses
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