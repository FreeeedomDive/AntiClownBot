using System.Collections.Generic;
using AntiClownBot.Models.User.Inventory.Items;

namespace AntiClownBot.Models.User.Inventory
{
    public static class AllItems
    {
        public static IEnumerable<Item> GetAllItems()
        {
            var result = new List<Item>
            {
                new CatWife(),
                new CommunismPoster(),
                new DogWife(),
                new Gigabyte(),
                new JadeRod(),
                new LootBox(),
                new RiceBowl()
            };

            return result;
        }
    }
}