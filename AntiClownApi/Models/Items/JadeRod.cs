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

        public static implicit operator DbItem(JadeRod item) => new()
        {
            Id = item.Id,
            Rarity = item.Rarity,
            ItemType = item.ItemType,
            Price = item.Price,
            Name = item.Name,
            ItemStats = new DbItemStats()
            {
                JadeRodLength = item.Length,
                JadeRodThickness = item.Thickness
            }
        };
    }
}