using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Models.Classes.Items;

namespace AntiClownBotApi.Database.DBModels.DbItems
{
    public class DbShopItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        // foreign key
        public Guid ShopId { get; set; }
        public DbUserShop Shop { get; set; }

        public string Name { get; set; }
        public Rarity Rarity { get; set; }
        public int Price { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsOwned { get; set; }

        public static DbShopItem GenerateNewShopItem(DbUserShop shop)
        {
            var rarity = Utility.GenerateRarity();
            return new DbShopItem()
            {
                Name = StringConstants
                    .GoodItemNames
                    .SelectMany(item => Enumerable.Repeat(item, 5))
                    .Union(StringConstants.BadItemNames)
                    .SelectRandomItem(),
                Rarity = rarity,
                Price = Utility.Prices[rarity],
                Shop = shop,
                IsOwned = false,
                IsRevealed = false
            };
        }
    }
}