using System;
using AntiClownBotApi.Models.Classes;

namespace AntiClownBotApi.Models
{
    public class User
    {
        public ulong Id { get; set; }
        public string DiscordUsername { get; set; }
        public int SocialRating { get; set; }
        public DateTime NextTribute { get; set; }
        public Inventory Inventory { get; set; }
    }
}