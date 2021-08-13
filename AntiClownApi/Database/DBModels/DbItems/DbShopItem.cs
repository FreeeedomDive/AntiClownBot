using System;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbShopItem
    {
        public Guid Id { get; set; }
        
        // foreign key
        public DbUserShop Shop { get; set; }
        
        public string Name { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsRevealed { get; set; }
    }
}