using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes.Items
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

        public static implicit operator DbItem(RiceBowl item) => new()
        {
            Id = item.Id,
            Rarity = item.Rarity,
            ItemType = item.ItemType,
            Price = item.Price,
            Name = item.Name,
            ItemStats = new DbItemStats()
            {
                RiceNegativeRangeExtend = item.NegativeRangeExtend,
                RicePositiveRangeExtend = item.PositiveRangeExtend
            }
        };
    }
}