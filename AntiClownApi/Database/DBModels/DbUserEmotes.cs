using System.ComponentModel.DataAnnotations.Schema;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUserEmotes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        // foreign key to user stats
        public ulong StatsId { get; set; }
        public DbUserStats UserStats { get; set; }
        
        // foreign key to emote
        public ulong EmoteId { get; set; }
        public DbEmote Emote { get; set; }
        
        public int Count { get; set; }
    }
}