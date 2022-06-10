using System;
using System.Collections.Generic;
using ApiWrapper.Constants;

namespace ApiWrapper.Models.Items
{
    public class CatWife : BaseItem
    {
        public CatWife(Guid id) : base(id)
        {
        }

        public int AutoTributeChance { get; init; }

        public override string Name => StringConstants.CatWifeName;
        public override ItemType ItemType => ItemType.Positive;
        
        public override Dictionary<string, string> Description() => new()
        {
            {"Шанс на автоматическое подношение", $"{AutoTributeChance}%"}
        };
    }
}