using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;

namespace AntiClownBotApi.Models.Items
{
    public class DogWife: BaseItem
    {
        public DogWife(Guid id) : base(id)
        {
        }
        
        public override string Name => StringConstants.DogWifeName;
        public override ItemType ItemType => ItemType.Positive;

        public int LootBoxFindChance { get; init; }
        
        public static explicit operator DogWife(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.DogWifeName)) throw new ArgumentException("Item is not a dog wife");

            return new DogWife(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                LootBoxFindChance = item.ItemStats.CatAutoTributeChance
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
                DogLootBoxFindChance = LootBoxFindChance
            };

            return item;
        }
    }
}