using System.Collections.Generic;
using System.IO;
using AntiClownBot;
using Newtonsoft.Json;

namespace AntiClownBotApi.Migration
{
    public class Configuration
    {
        private const string FileName = "config.json";

        public Dictionary<ulong, SocialRatingUser> Users;
        private static Configuration instance;

        public static Configuration GetConfiguration()
        {
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(FileName));
        }
    }
}