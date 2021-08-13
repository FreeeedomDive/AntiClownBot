using System;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Models.Items
{
    public abstract class BaseItem
    {
        public BaseItem(Guid id)
        {
            
        }
        
        public Guid Id { get; }
        public abstract string Name { get; }
        public abstract ItemType ItemType { get; } 
        public Rarity Rarity { get; init; }
        public int Price { get; set; }
    }
}