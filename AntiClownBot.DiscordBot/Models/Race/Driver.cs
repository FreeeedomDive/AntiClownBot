namespace AntiClownDiscordBotVersion2.Models.Race
{
    public class Driver
    {
        public ulong DiscordId = 0;
        public string Username = ""; 
        public bool IsUser = false;
        public DriverModel DriverModel;

        // public DiscordEmoji UsableEmoji;

        public int StartPosition;
        
        public int CurrentLap;
        public int CurrentLapTime;
        public int TotalTime;
        public int BestLap;
        public int[] TimesPerSector;
        public int TotalSectorsPassed;
        
        public bool IsFinished;
    }
}