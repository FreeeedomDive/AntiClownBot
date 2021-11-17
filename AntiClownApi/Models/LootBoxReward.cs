using System.Collections.Generic;
using AntiClownBotApi.Models.Items;

namespace AntiClownBotApi.Models
{
    public class LootBoxReward
    {
        public int ScamCoinsReward { get; set; }
        public List<BaseItem> Items { get; set; } = new();
    }
}