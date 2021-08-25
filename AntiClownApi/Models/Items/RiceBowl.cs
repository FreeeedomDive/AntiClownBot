using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public class RiceBowl : BaseItem
    {
        public RiceBowl(Guid id) : base(id)
        {
        }

        public int NegativeRangeExtend { get; init; }
        public int PositiveRangeExtend { get; init; }

        public override string Name => StringConstants.RiceBowlName;
        public override ItemType ItemType => ItemType.Positive;

        public static explicit operator RiceBowl(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.RiceBowlName)) throw new ArgumentException("Item is not a rice bowl");

            return new RiceBowl(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                NegativeRangeExtend = item.ItemStats.RiceNegativeRangeExtend,
                PositiveRangeExtend = item.ItemStats.RicePositiveRangeExtend
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
                RiceNegativeRangeExtend = NegativeRangeExtend,
                RicePositiveRangeExtend = PositiveRangeExtend
            };

            return item;
        }
    }
}