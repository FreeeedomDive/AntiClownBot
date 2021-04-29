using System;
using System.Collections.Generic;
using System.Linq;
using AntiClownBot.Models.BlackJack;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace AntiClownBot
{
    public static class Utility
    {
        public static DiscordClient Client;
        
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items) => items.OrderBy(item => Guid.NewGuid());

        public static Queue<T> WithoutItem<T>(this Queue<T> queue, T removableItem)
        {
            var newQueue = new Queue<T>();
            foreach (var item in queue.Where(item => !item.Equals(removableItem)))
            {
                newQueue.Enqueue(item);
            }

            return newQueue;
        }
        
        public static string ItemToString(InventoryItem item)
        {
            return item switch
            {
                InventoryItem.CatWife => "кошка-жена",
                InventoryItem.DogWife => "собака-жена",
                InventoryItem.RiceBowl => "рис миска",
                InventoryItem.Gigabyte => "гигабайт интернет",
                InventoryItem.None => "",
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
        }

        public static int CardToInt(Card card)
        {
            return card switch
            {
                Card.Two => 2,
                Card.Three => 3,
                Card.Four => 4,
                Card.Five => 5,
                Card.Six => 6,
                Card.Seven => 7,
                Card.Eight => 8,
                Card.Nine => 9,
                Card.Ten => 10,
                Card.Jack => 10,
                Card.Queen => 10,
                Card.King => 10,
                Card.Ace => 11,
                _ => throw new ArgumentOutOfRangeException(nameof(card), card, null)
            };
        }

        public static string StringEmoji(string emoji) => $"{DiscordEmoji.FromName(Client, emoji)}";
        public static DiscordEmoji Emoji(string emoji) => DiscordEmoji.FromName(Client, emoji);

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