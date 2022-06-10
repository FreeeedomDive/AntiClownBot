using System;

namespace ApiWrapper.Models.Items.Shop
{
    public class ShopItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsRevealed { get; set; }
    }
}