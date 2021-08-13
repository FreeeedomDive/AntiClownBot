using System;
using System.Collections.Generic;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbUserShop
    {
        public Guid Id { get; set; }
        
        // foreign key
        public ulong UserId { get; set; }
        public DbUser User { get; set; }
        
        public List<DbShopItem> Items { get; set; }
        public int ReRollPrice { get; set; }
        public int ItemReveals { get; set; }
    }
}