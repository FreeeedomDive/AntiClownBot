using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Models.Items;

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
            var possibleItems = new List<string>();
            foreach (var goodItem in StringConstants.GoodItemNames)
            {
                possibleItems.AddRange(Enumerable.Repeat(goodItem, 5));
            }

            possibleItems.AddRange(StringConstants.BadItemNames);

            return new DbShopItem()
            {
                Name = possibleItems.SelectRandomItem(),
                Rarity = rarity,
                Price = Utility.Prices[rarity],
                Shop = shop,
                IsOwned = false,
                IsRevealed = false
            };
        }
    }
}