namespace AntiClownDiscordBotVersion2.Models.Lohotron
{
    public class CreditsLohotronPrize : ILohotronPrize
    {
        public string Name => "Credits";
        public int Count;

        public CreditsLohotronPrize(int count)
        {
            Count = count;
        }
    }
}