using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AntiClownBotApi.Database.DBModels
{
    public class DbEmote
    {
        [Key]
        public ulong Id { get; set; }
        
        public string Name { get; set; }

        public List<DbUserEmotes> EmoteStats { get; set; } = new();
    }
}