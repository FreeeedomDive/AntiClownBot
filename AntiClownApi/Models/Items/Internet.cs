using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes.Items
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
                Gigabytes = item.ItemStats.InternetSpeed,
                Ping = item.ItemStats.InternetPing
            };
        }

        public static implicit operator DbItem(Internet item) => new()
        {
            Id = item.Id,
            Rarity = item.Rarity,
            ItemType = item.ItemType,
            Price = item.Price,
            Name = item.Name,
            ItemStats = new DbItemStats()
            {
                InternetSpeed = item.Speed,
                InternetGigabytes = item.Gigabytes,
                InternetPing = item.Ping
            }
        };
    }
}