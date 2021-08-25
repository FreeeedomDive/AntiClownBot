using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbUserStats
    {
        // primary key
        [Key]
        public ulong UserId { get; set; }
        public DbUser User { get; set; }
        
        public int TributeCount { get; set; }
        public int WrittenMessagesCount { get; set; }
        public int JoinedPartyCount { get; set; }
        
        public List<DbUserEmotes> UsedEmotes { get; set; } = new();
    }
}