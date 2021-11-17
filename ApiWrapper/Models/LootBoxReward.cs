using System.Collections.Generic;
using ApiWrapper.Models.Items;

namespace ApiWrapper.Models
{
    public class LootBoxReward
    {
        public int ScamCoinsReward { get; set; }
        public List<BaseItem> Items { get; set; } = new();
    }
}