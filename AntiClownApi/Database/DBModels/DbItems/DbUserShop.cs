using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AntiClownBotApi.Constants;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbUserShop
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        // foreign key
        public ulong UserId { get; set; }
        public DbUser User { get; set; }

        public List<DbShopItem> Items { get; set; }
        public int ReRollPrice { get; set; }
        public int FreeItemReveals { get; set; }

        public static DbUserShop GenerateNewShopForUser(ulong userId)
        {
            var shop = new DbUserShop()
            {
                UserId = userId,
                FreeItemReveals = 1,
                ReRollPrice = NumericConstants.DefaultReRollPrice,
                Items = new List<DbShopItem>()
            };
            for (var i = 0; i < NumericConstants.MaximumItemsInShop; i++)
            {
                shop.Items.Add(DbShopItem.GenerateNewShopItem(shop));
            }

            return shop;
        }

        public static DbUserShop GenerateNewItemsForShop(DbUserShop shop)
        {
            shop.Items.Clear();
            
            for (var i = 0; i < NumericConstants.MaximumItemsInShop; i++)
            {
                shop.Items.Add(DbShopItem.GenerateNewShopItem(shop));
            }

            return shop;
        }
    }
}