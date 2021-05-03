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
        None
    }

    public class SocialRatingUser
    {
        private static readonly List<InventoryItem> allItems = new List<InventoryItem>
            {InventoryItem.CatWife, InventoryItem.DogWife, InventoryItem.RiceBowl, InventoryItem.JadeRod};

        public ulong DiscordId;
        public string DiscordUsername;
        public int SocialRating;
        public readonly Dictionary<InventoryItem, int> UserItems;
        public DateTime NextTribute;

        public SocialRatingUser(ulong id, string name)
        {
            DiscordId = id;
            DiscordUsername = name;
            SocialRating = 500;
            NextTribute = DateTime.MinValue;
            UserItems = new Dictionary<InventoryItem, int>
            {
                {InventoryItem.CatWife, 0},
                {InventoryItem.DogWife, 0},
                {InventoryItem.RiceBowl, 0},
                {InventoryItem.Gigabyte, 0},
                {InventoryItem.JadeRod, 0}
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
                var newItem = allItems[Randomizer.GetRandomNumberBetween(0, allItems.Count)];
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
                var deletableItem = allItems[Randomizer.GetRandomNumberBetween(0, allItems.Count)];
                if (!UserItems.TryGetValue(deletableItem, out var value) || value <= 0) continue;

                UserItems[deletableItem]--;
                deletedItems.Add(deletableItem);
            }

            return deletedItems;
        }

        public bool HasDodgedPidor()
        {
            const int pidorDodgeChanceByOneDogWife = 5;
            return Randomizer.GetRandomNumberBetween(0, 100) <
                   pidorDodgeChanceByOneDogWife * UserItems[InventoryItem.DogWife];
        }

        public void UpdateCooldown()
        {
            const double cooldownDecreaseByOneGigabyteItem = 0.1;
            var cooldown = 60 * 60 * 1000d;

            const double cooldownDecreaseChanceByOneGigabyte = 5;
            for (var i = 0; i < UserItems[InventoryItem.Gigabyte]; i++)
            {
                // за каждый гигабайт 5% шанс уменьшить cock на 10%
                if (Randomizer.GetRandomNumberBetween(0, 100) < cooldownDecreaseChanceByOneGigabyte)
                {
                    cooldown *= 1 - cooldownDecreaseByOneGigabyteItem;
                }
            }

            const int cooldownIncreaseChanceByOneJade = 2;
            for (var i = 0; i < UserItems[InventoryItem.JadeRod]; i++)
            {
                // за каждый стержень 2% шанс увеличить cock в 2 раза
                if (Randomizer.GetRandomNumberBetween(0, 100) < cooldownIncreaseChanceByOneJade)
                {
                    cooldown *= 2;
                }
            }

            NextTribute = DateTime.Now.AddMilliseconds(cooldown);
        }

        public bool IsCooldownPassed()
        {
            return DateTime.Now > NextTribute;
        }
    }
}