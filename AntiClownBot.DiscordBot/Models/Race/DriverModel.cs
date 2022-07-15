using Newtonsoft.Json;

namespace AntiClownDiscordBotVersion2.Models.Race
{
    public class DriverModel
    {
        [JsonProperty("Name")]
        public string Name;
        [JsonProperty("ShortName")]
        public string ShortName;
        [JsonProperty("Points")]
        public int Points;
        [JsonProperty("CorneringStat")]
        public float CorneringStat;
        [JsonProperty("AccelerationStat")]
        public float AccelerationStat;
        [JsonProperty("BreakingStat")]
        public float BreakingStat;
    }
}