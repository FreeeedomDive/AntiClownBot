using System;
using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUserEmotes
    {
        [Key]
        public int Id { get; set; }
        
        // foreign key to user stats
        public DbUserStats UserStats { get; set; }
        
        // foreign key to emote
        public DbEmote Emote { get; set; }
        
        public int Count { get; set; }
    }
}