using System;
using AntiClownBotApi.Constants;
using AntiClownBotApi.Database.DBModels.DbItems;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models.Classes.Items
{
    public class JadeRod : BaseItem
    {
        public JadeRod(Guid id) : base(id)
        {
        }

        public override string Name => StringConstants.JadeRodName;
        public override ItemType ItemType => ItemType.Negative;

        public int Length { get; init; }
        public int Thickness { get; init; }

        public static explicit operator JadeRod(DbItem item)
        {
            if (!item.Name.Equals(StringConstants.JadeRodName))
                throw new ArgumentException("Item is not a jade rod");

            return new JadeRod(item.Id)
            {
                Rarity = item.Rarity,
                Price = item.Price,
                Length = item.ItemStats.JadeRodLength,
                Thickness = item.ItemStats.JadeRodThickness
            };
        }

        public override DbItem ToDbItem()
        {
            var item = new DbItem()
            {
                Id = Id,
                Name = Name,
                Rarity = Rarity,
                ItemType = ItemType,
                Price = Price
            };
            item.ItemStats = new DbItemStats()
            {
                Item = item,
                ItemId = item.Id,
                JadeRodLength = Length,
                JadeRodThickness = Thickness
            };

            return item;
        }
    }
}