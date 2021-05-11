using System;
using System.Collections.Generic;

namespace AntiClownBot
{
    public enum InventoryItem
    {
        CatWife,
        DogWife,
        RiceBowl,
        Gigabyte,
        JadeRod,
        CommunismPoster,
        None
    }

    public class SocialRatingUser
    {
        private static readonly List<InventoryItem> allItems = new List<InventoryItem>
            {InventoryItem.CatWife, InventoryItem.DogWife, InventoryItem.RiceBowl, InventoryItem.JadeRod, InventoryItem.CommunismPoster};

        public ulong DiscordId;
        public string DiscordUsername;
        public int SocialRating;
        public readonly Dictionary<InventoryItem, int> UserItems;
        public DateTime NextTribute;

        public SocialRatingUser(ulong id, string name)
        {
            DiscordId = id;
            DiscordUsername = name;
            SocialRating = Constants.DefaultSocialRating;
            NextTribute = DateTime.MinValue;
            UserItems = new Dictionary<InventoryItem, int>
            {
                {InventoryItem.CatWife, 0},
                {InventoryItem.DogWife, 0},
                {InventoryItem.RiceBowl, 0},
                {InventoryItem.Gigabyte, 0},
                {InventoryItem.JadeRod, 0},
                {InventoryItem.CommunismPoster, 0}
            };
        }

        public List<InventoryItem> IncreaseRating(int rating)
        {
            var oldRating = SocialRating;
            SocialRating += rating;
            var diff = SocialRating / 1000 - oldRating / 1000;

            var items = new List<InventoryItem>();

            if (diff == 0)
                return items;
            for (var i = 0; i < diff; i++)
            {
                var newItem = allItems.SelectRandomItem();
                if (!UserItems.ContainsKey(newItem))
                {
                    UserItems.Add(newItem, 0);
                }

                UserItems[newItem]++;
                items.Add(newItem);
            }

            return items;
        }

        public List<InventoryItem> DecreaseRating(int rating)
        {
            var oldRating = SocialRating;
            SocialRating = Math.Max(SocialRating - rating, 0);
            var diff = oldRating / 1000 - SocialRating / 1000;

            var deletedItems = new List<InventoryItem>();
            if (diff == 0)
                return deletedItems;

            while (deletedItems.Count != diff)
            {
                var deletableItem = allItems.SelectRandomItem();
                if (!UserItems.TryGetValue(deletableItem, out var value) || value <= 0) continue;

                UserItems[deletableItem]--;
                deletedItems.Add(deletableItem);
            }

            return deletedItems;
        }

        public bool HasDodgedPidor()
        {
            return Randomizer.GetRandomNumberBetween(0, 100) <
                   Utility.LogarithmicDistribution(Constants.LogarithmicDistributionStartValueForDogWife,
                       UserItems[InventoryItem.DogWife]);
        }

        public (int, int) UpdateCooldown()
        {
            var cooldown = Constants.DefaultCooldown;

            var gigabyteWorked = 0;
            var jadeRodWorked = 0;

            for (var i = 0; i < UserItems[InventoryItem.Gigabyte]; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) <
                      Constants.CooldownDecreaseChanceByOneGigabyte)) continue;
                gigabyteWorked++;
                cooldown *= 1 - Constants.CooldownDecreaseByOneGigabyteItem;
            }

            for (var i = 0; i < UserItems[InventoryItem.JadeRod]; i++)
            {
                if (!(Randomizer.GetRandomNumberBetween(0, 100) < Constants.CooldownIncreaseChanceByOneJade)) continue;
                jadeRodWorked++;
                cooldown *= Constants.CooldownIncreaseByOneJade;
            }

            NextTribute = DateTime.Now.AddMilliseconds(cooldown);
            return (gigabyteWorked, jadeRodWorked);
        }

        public bool IsCooldownPassed()
        {
            return DateTime.Now > NextTribute;
        }

        public override string ToString()
        {
            return $"{DiscordUsername} {SocialRating}";
        }
    }
}