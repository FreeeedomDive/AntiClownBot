using System;
using DSharpPlus.EventArgs;

namespace AntiClownBot
{
    public static class Utility
    {
        public static string ItemToString(InventoryItem item)
        {
            switch (item)
            {
                case InventoryItem.CatWife:
                    return "кошка-жена";
                case InventoryItem.DogWife:
                    return "собака-жена";
                case InventoryItem.RiceBowl:
                    return "рис миска";
                case InventoryItem.Gigabyte:
                    return "гигабайт интернет";
                case InventoryItem.None:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item, null);
            }
        }

        public static int GetTimeDiff(DateTime time1, DateTime time2)
        {
            var diff = time1 - time2;
            return (int) Math.Abs(diff.TotalMinutes);
        }
        
        public static async void IncreaseRating(Configuration config, SocialRatingUser user, int rating, MessageCreateEventArgs e)
        {
            var items = user.IncreaseRating(rating);
            foreach (var item in items)
            {
                await e.Message.RespondAsync($"Ты получаешь {ItemToString(item)}!");
            }

            config.Save();
        }

        public static async void DecreaseRating(Configuration config, SocialRatingUser user, int rating, MessageCreateEventArgs e)
        {
            var items = user.DecreaseRating(rating);
            foreach (var item in items)
            {
                await e.Message.RespondAsync($"Ты теряешь {ItemToString(item)}!");
            }

            config.Save();
        }
    }
}