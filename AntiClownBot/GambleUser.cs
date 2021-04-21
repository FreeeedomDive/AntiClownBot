namespace AntiClownBot
{
    public class GambleUser
    {
        public SocialRatingUser User;
        public int Bet;

        public GambleUser(SocialRatingUser user)
        {
            User = user;
            Bet = 0;
        }

        public void IncreaseBet(int bet)
        {
            Bet += bet;
        }
    }
}