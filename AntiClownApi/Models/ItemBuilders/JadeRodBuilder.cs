using System;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Models.ItemBuilders
{
    public class JadeRodBuilder: ItemBuilder
    {
        
        
        public JadeRodBuilder(Guid id, Rarity rarity, int price)
        {
            Id = id;
            Rarity = rarity;
            Price = price;
        }
    }
}