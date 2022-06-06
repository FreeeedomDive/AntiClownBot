using AntiClownApiClient.Dto.Models.Items;

namespace AntiClownApiClient.Dto.Responses.ShopResponses
{
    public class ShopItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsOwned { get; set; }
    }
}