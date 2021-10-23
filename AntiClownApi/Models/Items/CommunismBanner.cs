using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public class CommunismBanner : BaseItem
    {
        public CommunismBanner(Guid id) : base(id)
        {
        }

        public override string Name => StringConstants.CommunismBannerName;
        public override ItemType ItemType => ItemType.Negative;
        
        public int DivideChance { get; init; }
        public int StealChance { get; init; }

        public static explicit operator CommunismBanner(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.CommunismBannerName)) throw new ArgumentException("Item is not a communism banner");

            return new CommunismBanner(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                IsActive = item.IsActive,
                DivideChance = item.ItemStats.CommunismDivideChance,
                StealChance = item.ItemStats.CommunismStealChance
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
                CommunismDivideChance = DivideChance,
                CommunismStealChance = StealChance
            };

            return item;
        }
    }
}