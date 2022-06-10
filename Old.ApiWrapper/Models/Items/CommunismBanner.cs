using System;
using System.Collections.Generic;
using ApiWrapper.Constants;

namespace ApiWrapper.Models.Items
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

        public override Dictionary<string, string> Description() => new()
        {
            {
                "Шанс разделить награду за подношение с другим владельцем плаката",
                $"{DivideChance}%"
            },
            {
                "Приоритет стащить чужое подношение (если у него сработал плакат)",
                $"{StealChance}"
            }
        };
    }
}