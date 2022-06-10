using System;
using System.Collections.Generic;
using ApiWrapper.Constants;

namespace ApiWrapper.Models.Items
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

        public override Dictionary<string, string> Description() => new()
        {
            {"Шанс уменьшения кулдауна во время одной попытки", $"{Speed}%"},
            {"Попытки уменьшить кулдаун", $"{Gigabytes}"},
            {"Уменьшение кулдауна в процентах", $"{Ping}%"},
        };

    }
}