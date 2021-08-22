using System;
using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Models.Items
{
    public abstract class BaseItem
    {
        public BaseItem(Guid id)
        {
            Id = id;
        }
        
        public Guid Id { get; }
        public abstract string Name { get; }
        public abstract ItemType ItemType { get; } 
        public Rarity Rarity { get; init; }
        public int Price { get; set; }

        public abstract DbItem ToDbItem();
    }
}