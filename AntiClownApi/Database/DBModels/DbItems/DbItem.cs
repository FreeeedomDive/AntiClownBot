using System;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbItem
    {
        public Guid Id { get; set; }

        // foreign key
        public ulong UserId { get; set; }
        public DbUser User { get; set; }
        
        public string Name { get; set; }
        public ItemType ItemType { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsActive { get; set; }
        
        public DbItemStats ItemStats { get; set; }
    }
}