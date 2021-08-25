using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public class Internet : BaseItem
    {
        public Internet(Guid id) : base(id)
        {
        }

        public override string Name => StringConstants.InternetName;
        public override ItemType ItemType => ItemType.Positive;

        public int Gigabytes { get; init; }
        public int Speed { get; init; }
        public int Ping { get; init; }

        public static explicit operator Internet(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.InternetName)) throw new ArgumentException("Item is not an internet");

            return new Internet(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                Speed = item.ItemStats.InternetSpeed,
                Gigabytes = item.ItemStats.InternetGigabytes,
                Ping = item.ItemStats.InternetPing
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
                InternetSpeed = Speed,
                InternetGigabytes = Gigabytes,
                InternetPing = Ping
            };

            return item;
        }
    }
}