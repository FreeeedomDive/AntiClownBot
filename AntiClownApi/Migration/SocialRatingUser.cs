using AntiClownBot.Models.User.Inventory;
using AntiClownBot.Models.User.Inventory.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntiClownBot.Models.User.Stats;

namespace AntiClownBot
{
    public class SocialRatingUser
    {
        public ulong DiscordId;
        public string DiscordUsername;
        public int SocialRating;
        public Dictionary<Item, int> Items;
        public UserStats Stats;

        public int NetWorth => SocialRating + Items.Keys
            .Where(item =>
                item.Equals(new DogWife()) ||
                item.Equals(new CatWife()) ||
                item.Equals(new Gigabyte()) ||
                item.Equals(new RiceBowl()) ||
                item.Equals(new LootBox()))
            .Select(key => Items[key] * key.Price).Sum();

        public DateTime NextTribute;

        public SocialRatingUser(ulong id, string name)
        {
            DiscordId = id;
            DiscordUsername = name;
            SocialRating = 1000;
            NextTribute = DateTime.MinValue;
            Items = AllItems.GetAllItems().ToDictionary(item => item, _ => 0);
            Stats = new UserStats();
        }

        public override string ToString()
        {
            return $"{DiscordUsername} {SocialRating}";
        }
    }
}