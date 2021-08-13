using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes.Items
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
                DivideChance = item.ItemStats.CommunismDivideChance,
                StealChance = item.ItemStats.CommunismStealChance
            };
        }

        public static implicit operator DbItem(CommunismBanner item) => new()
        {
            Id = item.Id,
            Rarity = item.Rarity,
            ItemType = item.ItemType,
            Price = item.Price,
            Name = item.Name,
            ItemStats = new DbItemStats()
            {
                CommunismDivideChance = item.DivideChance,
                CommunismStealChance = item.StealChance
            }
        };
    }
}