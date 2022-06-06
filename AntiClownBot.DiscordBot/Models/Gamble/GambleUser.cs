namespace AntiClownDiscordBotVersion2.Models.Gamble
{
    public class GambleUser
    {
        public ulong DiscordId;
        public int Bet;

        public GambleUser(ulong id)
        {
            DiscordId = id;
            Bet = 0;
        }

        public void IncreaseBet(int bet)
        {
            Bet += bet;
        }
    }
}