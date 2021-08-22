using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public class CatWife : BaseItem
    {
        public CatWife(Guid id) : base(id)
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

        public override DbItem ToDbItem()
        {
            var item = new DbItem()
            {
                Id = Id,
                Rarity = Rarity,
                ItemType = ItemType,
                Price = Price,
                Name = Name
            };
            
            item.ItemStats = new DbItemStats()
            {
                Item = item,
                ItemId = item.Id,
                CatAutoTributeChance = AutoTributeChance
            };

            return item;
        }
    }
}