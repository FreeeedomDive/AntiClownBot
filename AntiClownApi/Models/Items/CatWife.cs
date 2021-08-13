using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes.Items
{
    public class CatWife: BaseItem
    {
        public CatWife(Guid id): base(id)
        {
        }
        
        public int AutoTributeChance { get; init; }

        public override string Name => StringConstants.CatWifeName;
        public override ItemType ItemType => ItemType.Positive;

        public static explicit operator CatWife(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.CatWifeName)) throw new ArgumentException("Item is not a cat wife");

            return new CatWife(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                AutoTributeChance = item.ItemStats.CatAutoTributeChance
            };
        }

        public static implicit operator DbItem(CatWife item) => new()
        {
            Id = item.Id,
            Rarity = item.Rarity,
            ItemType = item.ItemType,
            Price = item.Price,
            Name = item.Name,
            ItemStats = new DbItemStats()
            {
                CatAutoTributeChance = item.AutoTributeChance
            }
        };
    }
}